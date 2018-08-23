﻿using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using System;

namespace OfficeDevPnP.Core.Tests.Framework.ProvisioningTemplates
{
    [TestClass]
    public class ProvisioningTests
    {

        [TestMethod]
        public void GetGroupInfoTest()
        {
            using (var context = TestCommon.CreateClientContext())
            {
                OfficeDevPnP.Core.Sites.SiteCollection.GetGroupInfo(context, "demo1").GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void ProvisionTenantTemplate()
        {
            //var resourceFolder = string.Format(@"{0}\..\..\Resources\Templates",
            //  AppDomain.CurrentDomain.BaseDirectory);
            //XMLTemplateProvider provider = new XMLFileSystemTemplateProvider(resourceFolder, "");

            Guid siteGuid = Guid.NewGuid();
            int siteId = siteGuid.GetHashCode();
            var template = new ProvisioningTemplate();
            template.Id = "TestTemplate";
            template.Lists.Add(new ListInstance()
            {
                Title = "Testlist",
                TemplateType = 100,
                Url = "lists/testlist"
            });


            Provisioning sequenceTemplate = new Provisioning();

            sequenceTemplate.Templates.Add(template);

            sequenceTemplate.Parameters.Add("Test", "test");

            var sequence = new ProvisioningSequence();
            var teamSite1 = new TeamSiteCollection()
            {
                //  Alias = $"prov-1-{siteId}",
                Alias = "prov-1",
                Description = "prov-1",
                DisplayName = "prov-1",
                IsHubSite = false,
                IsPublic = false,
                Title = "prov-1",
            };
            teamSite1.Templates.Add("TestTemplate");

            var subsite = new TeamNoGroupSubSite()
            {
                Description = "Test Sub",
                Url = "testsub1",
                Language = "1033",
                TimeZoneId = 4,
                Title = "Test Sub",
                UseSamePermissionsAsParentSite = true
            };
            subsite.Templates.Add("TestTemplate");
            teamSite1.Sites.Add(subsite);

            sequence.SiteCollections.Add(teamSite1);

            var teamSite2 = new TeamSiteCollection()
            {
                Alias = $"prov-2-{siteId}",
                Description = "prov-2",
                DisplayName = "prov-2",
                IsHubSite = false,
                IsPublic = false,
                Title = "prov-2"
            };
            teamSite2.Templates.Add("TestTemplate");

            sequence.SiteCollections.Add(teamSite2);

            sequenceTemplate.Sequences.Add(sequence);


            using (var tenantContext = TestCommon.CreateTenantClientContext())
            {
                var applyingInformation = new ProvisioningTemplateApplyingInformation();
                applyingInformation.ProgressDelegate = (message, step, total) =>
                {
                    if (message != null)
                    {


                    }
                };

                var tenant = new Tenant(tenantContext);

                tenant.ApplyTemplate(sequenceTemplate, applyingInformation);
            }
        }
    }
}
