using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Benchmarks.Serializers
{
    internal static class DataGenerator
    {
        internal static T Generate<T>()
        {
            if (typeof(T) == typeof(LoginViewModel))
                return (T)(object)CreateLoginViewModel();
            if (typeof(T) == typeof(Location))
                return (T)(object)CreateLocation();
            if (typeof(T) == typeof(IndexViewModel))
                return (T)(object)CreateIndexViewModel();
            if (typeof(T) == typeof(MyEventsListerViewModel))
                return (T)(object)CreateMyEventsListerViewModel();

            throw new NotImplementedException();
        }

        private static LoginViewModel CreateLoginViewModel()
            => new LoginViewModel
            {
                Email = "name.familyname@not.com",
                Password = "abcdefgh123456!@",
                RememberMe = true
            };

        private static Location CreateLocation()
            => new Location
            {
                Id = 1234,
                Address1 = "The Street Name",
                Address2 = "20/11",
                City = "The City",
                State = "The State",
                PostalCode = "abc-12",
                Name = "Nonexisting",
                PhoneNumber = "+0 11 222 333 44",
                Country = "The Greatest"
            };

        private static IndexViewModel CreateIndexViewModel()
            => new IndexViewModel
            {
                IsNewAccount = false,
                FeaturedCampaign = new CampaignSummaryViewModel
                {
                    Description = "Very nice campaing",
                    Headline = "The Headline",
                    Id = 234235,
                    OrganizationName = "The Company XYZ",
                    ImageUrl = "https://www.dotnetfoundation.org/theme/img/carousel/foundation-diagram-content.png",
                    Title = "Promoting Open Source"
                },
                ActiveOrUpcomingEvents = Enumerable.Repeat(
                    new ActiveOrUpcomingEvent
                    {
                        Id = 10,
                        CampaignManagedOrganizerName = "Name FamiltyName",
                        CampaignName = "The very new campaing",
                        Description = "The .NET Foundation works with Microsoft and the broader industry to increase the exposure of open source projects in the .NET community and the .NET Foundation. The .NET Foundation provides access to these resources to projects and looks to promote the activities of our communities.",
                        EndDate = DateTime.UtcNow.AddYears(1),
                        Name = "Just a name",
                        ImageUrl = "https://www.dotnetfoundation.org/theme/img/carousel/foundation-diagram-content.png",
                        StartDate = DateTime.UtcNow
                    },
                    count: 20).ToList()
            };

        private static MyEventsListerViewModel CreateMyEventsListerViewModel()
            => new MyEventsListerViewModel
            {
                CurrentEvents = Enumerable.Repeat(CreateMyEventsListerItem(), 3).ToList(),
                FutureEvents = Enumerable.Repeat(CreateMyEventsListerItem(), 9).ToList(),
                PastEvents = Enumerable.Repeat(CreateMyEventsListerItem(), 60).ToList() // usually  there is a lot of historical data
            };

        private static MyEventsListerItem CreateMyEventsListerItem()
            => new MyEventsListerItem
            {
                Campaign = "A very nice campaing",
                EndDate = DateTime.UtcNow.AddDays(7),
                EventId = 321,
                EventName = "wonderful name",
                Organization = "Local Animal Shelter",
                StartDate = DateTime.UtcNow.AddDays(-7),
                TimeZone = TimeZoneInfo.Utc.DisplayName,
                VolunteerCount = 15,
                Tasks = Enumerable.Repeat(
                    new MyEventsListerItemTask
                    {
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(1),
                        Name = "A very nice task to have"
                    }, 4).ToList()
            };
    }

    // the view models come from a real world app called "AllReady"
    [Serializable]
    [ProtoContract]
    public class LoginViewModel
    {
        [ProtoMember(1)]public string Email { get; set; }
        [ProtoMember(2)]public string Password { get; set; }
        [ProtoMember(3)]public bool RememberMe { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class Location
    {
        [ProtoMember(1)]public int Id { get; set; }
        [ProtoMember(2)]public string Address1 { get; set; }
        [ProtoMember(3)]public string Address2 { get; set; }
        [ProtoMember(4)]public string City { get; set; }
        [ProtoMember(5)]public string State { get; set; }
        [ProtoMember(6)]public string PostalCode { get; set; }
        [ProtoMember(7)]public string Name { get; set; }
        [ProtoMember(8)]public string PhoneNumber { get; set; }
        [ProtoMember(9)]public string Country { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class ActiveOrUpcomingCampaign
    {
        [ProtoMember(1)]public int Id { get; set; }
        [ProtoMember(2)]public string ImageUrl { get; set; }
        [ProtoMember(3)]public string Name { get; set; }
        [ProtoMember(4)]public string Description { get; set; }
        [ProtoMember(5)]public DateTimeOffset StartDate { get; set; }
        [ProtoMember(6)]public DateTimeOffset EndDate { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class ActiveOrUpcomingEvent
    {
        [ProtoMember(1)]public int Id { get; set; }
        [ProtoMember(2)]public string ImageUrl { get; set; }
        [ProtoMember(3)]public string Name { get; set; }
        [ProtoMember(4)]public string CampaignName { get; set; }
        [ProtoMember(5)]public string CampaignManagedOrganizerName { get; set; }
        [ProtoMember(6)]public string Description { get; set; }
        [ProtoMember(7)]public DateTimeOffset StartDate { get; set; }
        [ProtoMember(8)]public DateTimeOffset EndDate { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class CampaignSummaryViewModel
    {
        [ProtoMember(1)]public int Id { get; set; }
        [ProtoMember(2)]public string Title { get; set; }
        [ProtoMember(3)]public string Description { get; set; }
        [ProtoMember(4)]public string ImageUrl { get; set; }
        [ProtoMember(5)]public string OrganizationName { get; set; }
        [ProtoMember(6)]public string Headline { get; set; }
    }

    [Serializable]
    [ProtoContract]
    public class IndexViewModel
    {
        [ProtoMember(1)]public List<ActiveOrUpcomingEvent> ActiveOrUpcomingEvents { get; set; }
        [ProtoMember(2)]public CampaignSummaryViewModel FeaturedCampaign { get; set; }
        [ProtoMember(3)]public bool IsNewAccount { get; set; }
        public bool HasFeaturedCampaign => FeaturedCampaign != null;
    }

    [Serializable]
    [ProtoContract]
    public class MyEventsListerViewModel
    {
        // the orginal type defined these fields as IEnumerable,
        // but XmlSerializer failed to serialize them with "cannot serialize member because it is an interface" error
        [ProtoMember(1)]public List<MyEventsListerItem> CurrentEvents { get; set; } = new List<MyEventsListerItem>();
        [ProtoMember(2)]public List<MyEventsListerItem> FutureEvents { get; set; } = new List<MyEventsListerItem>();
        [ProtoMember(3)]public List<MyEventsListerItem> PastEvents { get; set; } = new List<MyEventsListerItem>();
    }

    [Serializable]
    [ProtoContract]
    public class MyEventsListerItem
    {
        [ProtoMember(1)]public int EventId { get; set; }
        [ProtoMember(2)]public string EventName { get; set; }
        [ProtoMember(3)]public DateTimeOffset StartDate { get; set; }
        [ProtoMember(4)]public DateTimeOffset EndDate { get; set; }
        [ProtoMember(5)]public string TimeZone { get; set; }
        [ProtoMember(6)]public string Campaign { get; set; }
        [ProtoMember(7)]public string Organization { get; set; }
        [ProtoMember(8)]public int VolunteerCount { get; set; }

        [ProtoMember(9)]public List<MyEventsListerItemTask> Tasks { get; set; } = new List<MyEventsListerItemTask>();
    }

    [Serializable]
    [ProtoContract]
    public class MyEventsListerItemTask
    {
        [ProtoMember(1)]public string Name { get; set; }
        [ProtoMember(2)]public DateTimeOffset? StartDate { get; set; }
        [ProtoMember(3)]public DateTimeOffset? EndDate { get; set; }

        public string FormattedDate
        {
            get
            {
                if (!StartDate.HasValue || !EndDate.HasValue)
                {
                    return null;
                }

                var startDateString = string.Format("{0:g}", StartDate.Value);
                var endDateString = string.Format("{0:g}", EndDate.Value);

                return string.Format($"From {startDateString} to {endDateString}");
            }
        }
    }
}