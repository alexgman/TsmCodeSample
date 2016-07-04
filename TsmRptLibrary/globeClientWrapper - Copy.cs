using System;
using System.Collections.Generic;

namespace TsmRptLibrary
{
    internal class AtlasClientWrapper : IAtlasClient

    {
        private AtlasClient _atlasClient;

        private AtlasClientWrapper(AtlasClient atlasClient)
        {
            this._atlasClient = atlasClient;
        }

        public void ApproveChangeSet(long changeSetId)
        {
            throw new NotImplementedException();
        }

        public HierarchyNodeInfo ConcretizeHierarchyNode(HierarchyNodeReference node)
        {
            throw new NotImplementedException();
        }

        public void ConfigureChannelFactory(Action<ChannelFactory<IAtlasService>> channelFactoryConfigurator)
        {
            throw new NotImplementedException();
        }

        public CreateChangeSetResponse CreateChangeSet(CreateChangeSetRequest request)
        {
            throw new NotImplementedException();
        }

        public ApplicationInfo GetApplication(ApplicationReference application)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ApplicationReference> GetApplications()
        {
            throw new NotImplementedException();
        }

        public ChangeSetInfo GetChangeSet(long changeSetId)
        {
            throw new NotImplementedException();
        }

        public GetChangeSetsResponse GetChangeSets(GetChangeSetsRequest request)
        {
            throw new NotImplementedException();
        }

        public HierarchyNodeInfo GetHierarchyNode(HierarchyNodeReference node)
        {
            throw new NotImplementedException();
        }

        public GetHierarchyNodesResponse GetHierarchyNodes(GetHierarchyNodesRequest request)
        {
            throw new NotImplementedException();
        }

        public GetValuesResponse GetSettingValues(GetValuesRequest request)
        {
            throw new NotImplementedException();
        }

        public string GetVersion()
        {
            throw new NotImplementedException();
        }

        public GetHierarchyNodesResponse QueryHierarchyNodes(QueryHierarchyNodesRequest request)
        {
            throw new NotImplementedException();
        }

        public void RejectChangeSet(long changeSetId)
        {
            throw new NotImplementedException();
        }

        public void SetUsernameCredentials(string username, string password)
        {
            throw new NotImplementedException();
        }

        public void SetWindowsCredentials(string username, string password, string domain)
        {
            this._atlasClient.SetWindowsCredentials(username, password, domain);
        }

        public SettingValueBindingCollection GetCurrentValuesNonStatic(HierarchyNodeReference node)
        {
            return this._atlasClient.GetCurrentValues(node);
        }
    }
}