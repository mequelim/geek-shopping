using AutoMapper;
using GeekShopping.CartAPI.Data.DTOs;
using GeekShopping.CartAPI.Infrastructure.Database;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartAPI.Repository
{
    /// <summary>
    /// Provides an implementation of the <see cref="GeekShopping.CartAPI.Repository.Interfaces.ICartRepository"/> interface to manage shopping cart operations, including
    /// retrieval, updates, and
    /// coupon management.
    /// </summary>
    /// <remarks>
    /// This class interacts with the database using the <see cref="GeekShopping.CartAPI.Infrastructure.Database.AppDbContext"/> and performs object mapping using
    /// <see cref="AutoMapper.IMapper"/>.
    /// </remarks>
    public class CartRepository(AppDbContext databaseContext, IMapper mapper) : ICartRepository
    {
        private readonly AppDbContext _databaseContext = databaseContext
                                                         ?? throw new ArgumentNullException(nameof(databaseContext));
        private readonly IMapper _mapper = mapper
                                           ?? throw new ArgumentNullException(nameof(mapper));

        // Methods:
        /// <summary>
        /// Retrieves the shopping cart associated with the specified user ID.
        /// </summary>
        /// <param name="id">The unique identifier representing the user whose cart is to be retrieved.</param>
        /// <returns>
        /// An instance of <see cref="GeekShopping.CartAPI.Data.DTOs.CartDto"/> containing the shopping cart details associated with the user.
        /// If the user does not have an existing cart, a new cart with default values is returned.
        /// </returns>
        public async Task<CartDto> FindCartByUserId(Guid id)
        {
            CartHeader? header = await _databaseContext.CartHeaders
                .FirstOrDefaultAsync((h) => h.UserId.Equals(id));

            Console.WriteLine($">>> header: {System.Text.Json.JsonSerializer.Serialize(header)}.");

            if(header is null) return new CartDto
            {
                CartHeader = new CartHeaderDto { UserId = id },
                CartDetails = []
            };

            List<CartDetail> details = await _databaseContext.CartDetails
                .Where((c) => c.CartHeaderId.Equals(header.Id))
                .Include((c) => c.Product)
                .ToListAsync();

            Console.WriteLine($">>> Details count: {details.Count}.");

            Cart cart = new()
            {
                CartHeader = header,
                CartDetails = details
            };

            return _mapper.Map<CartDto>(cart);
        }

        /// <summary>
        /// Saves a new cart or updates an existing cart in the database.
        /// </summary>
        /// <param name="cartDto">
        /// An instance of <see cref="GeekShopping.CartAPI.Data.DTOs.CartDto"/> representing the cart data transfer object containing the necessary information to create or update a cart.
        /// </param>
        /// <returns>A <see cref="GeekShopping.CartAPI.Data.DTOs.CartDto"/> instance containing the saved or updated cart information.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the cart details or product information is null or improperly mapped.</exception>
        public async Task<CartDto> SaveOrUpdateCart(CartDto cartDto)
        {
            //? Mapping DTO to Entity:
            Cart cart = _mapper.Map<Cart>(cartDto);

            //? Validating the mapping result:
            CartDetail mappedDetail = cart.CartDetails?.FirstOrDefault()
                                      ?? throw new InvalidOperationException("CartDetails is null or empty after mapping!");

            Product mappedProduct = mappedDetail.Product
                                    ?? throw new InvalidOperationException("Product is null after mapping. Check AutoMapper profile!");

            //? Checking if the product already exists:
            Product? product = await _databaseContext.Products
                .FirstOrDefaultAsync((p) => p.Id.Equals(cartDto.CartDetails.FirstOrDefault()!.ProductId));

            if(product is null)
            {
                _databaseContext.Products.Add(mappedProduct);
                await _databaseContext.SaveChangesAsync();
            }

            //? Checking if CartHeader exists:
            CartHeader? cartHeader = await _databaseContext.CartHeaders
                .AsNoTracking()
                .FirstOrDefaultAsync((h) => (cart.CartHeader != null) && (h.UserId.Equals(cart.CartHeader.UserId)));

            //? Creating CartHeader and CartDetails:
            if(cartHeader is null)
            {
                //? Creating CartHeader:
                if(cart.CartHeader is not null)
                {
                    _databaseContext.CartHeaders.Add(cart.CartHeader);
                    await _databaseContext.SaveChangesAsync();

                    //? Creating CartDetails:
                    cart.CartDetails?.FirstOrDefault()?.CartHeaderId = cart.CartHeader.Id;
                }

                mappedDetail.Product = null;
                _databaseContext.CartDetails.Add(mappedDetail);
            }
            else
            {
                //? Checking if CartDetails has the same product:
                CartDetail? cartDetail = await _databaseContext.CartDetails
                    .AsNoTracking()
                    .FirstOrDefaultAsync((p) => p.ProductId.Equals(mappedDetail.ProductId) && p.CartHeaderId.Equals(cartHeader.Id));

                if(cartDetail is null)
                {
                    //? Creating CartDetails:
                    if((cart.CartHeader is not null) && (cart.CartDetails is null)) mappedDetail.CartHeaderId = cartHeader.Id;

                    mappedDetail.Product = null;
                    _databaseContext.CartDetails.Add(mappedDetail);
                }
                else
                {
                    if(cart.CartDetails is not null)
                    {
                        mappedDetail.Product = null;
                        mappedDetail.Count += cartDetail.Count;
                        mappedDetail.Id = cartDetail.Id;
                        mappedDetail.CartHeaderId = cartDetail.CartHeaderId;
                        mappedDetail.Version = cartDetail.Version;  // Preserving version for concurrency token match.
                        _databaseContext.CartDetails.Update(mappedDetail);
                    }
                }
            }

            await _databaseContext.SaveChangesAsync();

            return _mapper.Map<CartDto>(cart);
        }

        /// <summary>
        /// Removes an item from the shopping cart identified by the specified cart details ID.
        /// If the cart contains only one item, the associated cart header is also removed.
        /// </summary>
        /// <param name="cartDetailsId">The unique identifier of the cart detail to be removed.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the removal operation was successful.
        /// </returns>
        public async Task<bool> RemoveFromCart(Guid cartDetailsId)
        {
            try
            {
                CartDetail? cartDetail = await _databaseContext.CartDetails
                    .FirstOrDefaultAsync((d) => d.Id.Equals(cartDetailsId));

                int total = _databaseContext.CartDetails
                    .Count((d) => d.CartHeaderId.Equals(cartDetail!.CartHeaderId));

                if(cartDetail is not null) _databaseContext.CartDetails.Remove(cartDetail);
                if(total.Equals(1))
                {
                    CartHeader? cartHeaderToRemove = await _databaseContext.CartHeaders
                        .FirstOrDefaultAsync((h) => h.Id.Equals(cartDetail!.CartHeaderId));

                    _databaseContext.CartHeaders.Remove(cartHeaderToRemove!);
                }

                await _databaseContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the shopping cart for a specified user by removing the cart header and all associated cart details from the database.
        /// </summary>
        /// <param name="userId">A <see cref="string"/> representing the unique identifier of the user whose cart should be cleared.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> returning <see cref="bool"/> indicating whether the cart was successfully cleared. Returns true if the operation succeeds, otherwise false.
        /// </returns>
        public async Task<bool> ClearCart(Guid userId)
        {
            CartHeader? cartHeader = await _databaseContext.CartHeaders
                .FirstOrDefaultAsync((c) => c.UserId.Equals(userId));

            if(cartHeader is not null)
            {
                _databaseContext.CartDetails.RemoveRange(
                    _databaseContext.CartDetails.Where((c) => c.CartHeaderId.Equals(cartHeader.Id))
                );
                _databaseContext.CartHeaders.Remove(cartHeader);
                await _databaseContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Applies a coupon to the shopping cart associated with the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose shopping cart the coupon will be applied to.</param>
        /// <param name="couponCode">The code of the coupon to be applied.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the coupon was successfully applied.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown when the method is not yet implemented.</exception>
        public async Task<bool> ApplyCoupon(Guid userId, string? couponCode)
        {
            CartHeader? cartHeader = await _databaseContext.CartHeaders
                .FirstOrDefaultAsync((c) => c.UserId.Equals(userId));

            if(cartHeader is not null)
            {
                cartHeader.CouponCode = couponCode;

                _databaseContext.CartHeaders.Update(cartHeader);
                await _databaseContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes an applied coupon from the shopping cart associated with the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart's coupon should be removed.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a boolean indicating whether the coupon was successfully removed.
        /// </returns>
        /// <exception cref="NotImplementedException">Thrown if the method is not yet implemented.</exception>
        public async Task<bool> RemoveCoupon(Guid userId)
        {
            CartHeader? cartHeader = await _databaseContext.CartHeaders
                .FirstOrDefaultAsync((c) => c.UserId.Equals(userId));

            if(cartHeader is not null)
            {
                cartHeader.CouponCode = null;

                _databaseContext.CartHeaders.Update(cartHeader);
                await _databaseContext.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}