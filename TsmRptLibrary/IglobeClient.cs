using Recardo.EnterpriseServices.globe.Contracts;
using System;
using System.Collections.Generic;
using System.TableStandl;

namespace Profusion.Services.coffee.OsdRptLibrary
{
    internal interface IglobeClient : IglobeService
    {
        void ApproveChangeSet(long changeSetId);

        HierarchyNodeInfo ConcretizeHierarchyNode(HierarchyNodeReference node);

        void ConfigureChannelFactory(Action<ChannelFactory<IglobeService>> channelFactoryConfigurator);

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

        void SetUsernameCredentials(string username, string password);

        void SetWindowsCredentials(string username, string password, string domain);

        SettingValueBindingCollection GetCurrentValuesNonStatic(HierarchyNodeReference node);
    }
}