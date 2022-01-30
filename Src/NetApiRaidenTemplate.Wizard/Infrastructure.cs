namespace NetApiRaidenTemplate.Wizard
{
    public enum IdentityOption
    {
        None,
        IdentityServer,
        IdentityCore
    }

    public static class ProjectKinds
    {
        public const string ProjectKindSolutionFolder = EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder;
        public const string OAProject = "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";
        public const string FolderItem = "6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C";
    }
}
