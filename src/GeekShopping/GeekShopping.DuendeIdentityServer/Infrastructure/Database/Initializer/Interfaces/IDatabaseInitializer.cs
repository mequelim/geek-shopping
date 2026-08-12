namespace GeekShopping.DuendeIdentityServer.Infrastructure.Database.Initializer.Interfaces
{
    public interface IDatabaseInitializer
    {
        /// <summary>
        /// Initializes the database for the application.
        /// </summary>
        /// <remarks>
        /// This method sets up the necessary database state, including creating required tables, roles, users, or seeding initial data.
        /// It is designed to prepare the database environment for application use and ensure it is in the expected state.
        /// </remarks>
        void Initialize();
    }
}