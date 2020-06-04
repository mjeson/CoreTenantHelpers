namespace BlitzkriegSoftware.Tenant.MongoProvider.Models
{
    /// <summary>
    /// Configuration: Mongo DB
    /// </summary>
    public class MongoConfiguration
    {

        /// <summary>
        /// Auth Mechanism
        /// </summary>
        public string AuthMechanism { get; set; } = "";

        /// <summary>
        /// Port
        /// </summary>
        public int Port { get; set; } = 27017;
        
        /// <summary>
        /// Host
        /// </summary>
        public string Host { get; set; } = "127.0.0.1";

        /// <summary>
        /// Database
        /// </summary>
        public string Database { get; set; } = "";

        /// <summary>
        /// Collection
        /// </summary>
        public string Collection { get; set; } = "";

        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = "";

        /// <summary>
        /// MongoDB Connection String
        /// </summary>
        /// <returns>(sic)</returns>
        public override string ToString()
        {
            if(string.IsNullOrWhiteSpace(this.Username))
            {
                return $"mongodb://{this.Host}:{this.Port}";
            }
            else
            {
                return $"mongodb://{this.Username}:{this.Password}@{this.Host}:{this.Port}";
            }
        }
    }
}
