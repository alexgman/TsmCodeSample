using eCardio.EnterpriseServices.Atlas.Contracts;
using System;
using System.Collections.Generic;

namespace TsmRptLibraryTests
{
    internal static class AtlasClientTesterExtensions
    {
        public static SettingValueBindingCollection GetCurrentValues(this IAtlasClientTester service, HierarchyNodeReference node)
        {
            throw new NotImplementedException();
        }
    }

    internal class WrapperMethod
    {
        private readonly IStaticWrapper _wrapper;

        public WrapperMethod(IStaticWrapper wrapper)
        {
            this._wrapper = wrapper;
        }

        public SettingValueBindingCollection SomeMethod(IAtlasClientTester service, HierarchyNodeReference node)
        {
            var value = this._wrapper.GetCurrentValues(service, node);

            return value;
        }
    }

    internal interface IStaticWrapper
    {
        SettingValueBindingCollection GetCurrentValues(IAtlasClientTester service, HierarchyNodeReference node);
    }

    internal class StaticWrapper : IStaticWrapper
    {
        public SettingValueBindingCollection GetCurrentValues(IAtlasClientTester service, HierarchyNodeReference node)
        {
            return AtlasClientTesterExtensions.GetCurrentValues(service, node);
        }
    }

    internal interface IAtlasClientTester : IAtlasServiceWrapper
    {
        void ApproveChangeSet(long changeSetId);

        HierarchyNodeInfo ConcretizeHierarchyNode(HierarchyNodeReference node);

        CreateChangeSetResponse CreateChangeSet(CreateChangeSetRequest request);

        ApplicationInfo GetApplication(ApplicationReference application);

        IEnumerable<ApplicationReference> GetApplications();

        ChangeSetInfo GetChangeSet(long changeSetId);

        GetChangeSetsResponse GetChangeSets(GetChangeSetsRequest request);

        HierarchyNodeInfo GetHierarchyNode(HierarchyNodeReference node);

        GetHierarchyNodesResponse GetHierarchyNodes(GetHierarchyNodesRequest request);

        GetValuesResponse GetSettingValues(GetValuesRequest request);

        string GetVersion();

        GetHierarchyNodesResponse QueryHierarchyNodes(QueryHierarchyNodesRequest request);

        void RejectChangeSet(long changeSetId);

        SettingValueBindingCollection GetCurrentValues(IAtlasClientTester service, HierarchyNodeReference node);
    }

    internal class AtlasClientTester : IAtlasService, IStaticWrapper, IAtlasClientTester
    {
        public void ApproveChangeSet(long changeSetId)
        {
            throw new NotImplementedException();
        }

        public HierarchyNodeInfo ConcretizeHierarchyNode(HierarchyNodeReference node)
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

        public SettingValueBindingCollection GetCurrentValues(IAtlasClientTester service, HierarchyNodeReference node)
        {
            throw new NotImplementedException();
        }
    }
}