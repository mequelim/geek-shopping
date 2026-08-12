namespace GeekShopping.ProductAPI.Shared.User
{
    /// <summary>
    /// Represents a static class containing predefined user roles.
    /// </summary>
    public static class Role
    {
        /// <summary>
        /// Represents the role of an administrator within the system.
        /// Used to identify and grant specific permissions to users with administrative privileges.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Represents the role of a client within the system.
        /// Used to designate users who engage with the system as customers for purchasing or interacting with offered products and services.
        /// </summary>
        public const string Client = "Customer";
    }
}