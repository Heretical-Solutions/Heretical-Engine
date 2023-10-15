namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Enumerates the possible sources of a resource.
    /// </summary>
    public enum EResourceSources
    {
        /// <summary>
        /// The resource is obtained from a scriptable settings file.
        /// </summary>
        SCRIPTABLE_SETTINGS_FILE,

        /// <summary>
        /// The resource is obtained using Unity's Resources.Load() method.
        /// </summary>
        UNITY_RESOURCES_LOAD,

        /// <summary>
        /// The resource is obtained from Unity's StreamingAssets folder.
        /// </summary>
        UNITY_STREAMING_ASSETS,

        /// <summary>
        /// The resource is obtained from cloud storage.
        /// </summary>
        CLOUD_STORAGE,

        /// <summary>
        /// The resource is obtained from local storage.
        /// </summary>
        LOCAL_STORAGE,

        /// <summary>
        /// The resource is obtained from a network source.
        /// </summary>
        NETWORK_OBTAINED,

        /// <summary>
        /// The resource is generated at runtime.
        /// </summary>
        RUNTIME_GENERATED
    }
}