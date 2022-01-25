namespace $safeprojectname$.Settings
{
    public record AppConfigurationSettings
    {
        /* $identityserver_feature$ start */
        public bool ExecuteIdentityServerSeedData { get; set; }
        /* $identityserver_feature$ end */
    }
}
