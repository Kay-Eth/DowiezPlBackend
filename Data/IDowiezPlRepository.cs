using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DowiezPlBackend.Enums;
using DowiezPlBackend.Models;

namespace DowiezPlBackend.Data
{
    public interface IDowiezPlRepository
    {
        Task<bool> SaveChangesAsync();

        void CreateCity(City city);
        Task<List<City>> GetCitiesAsync();
        Task<City> GetCityAsync(Guid cityId);
        Task<City> GetCityNotTrackedAsync(Guid cityId);
        Task<List<City>> GetCityDistrictsAsync(City city);
        void DeleteCity(City city);

        void CreateDemand(Demand demand);
        Task<List<Demand>> SearchDemandsAsync(
            AppUser excludeUser,
            ICollection<DemandCategory> categories,
            Guid? fromCityId,
            Guid? destinationCityId,
            Guid? limitedToGroupId
        );
        Task<List<Demand>> GetUserDemandsAsync(Guid userId);
        Task<Demand> GetDemandAsync(Guid demandId);
        Task<Demand> GetDemandNotTrackedAsync(Guid demandId);
        void DeleteDemand(Demand demand);

        void CreateGroup(Group group);
        Task<List<Group>> GetGroupsAsync();
        Task<Group> GetGroupAsync(Guid groupId);
        Task<Group> GetGroupNotTrackedAsync(Guid groupId);
        void DeleteGroup(Group group);

        void CreateMember(Member member);
        Task<Member> GetMemberAsync(Guid member);
        Task<Member> GetMemberAsync(Guid groupId, Guid userId);
        Task<Member> GetMemberNotTrackedAsync(Guid member);
        Task<List<Member>> GetUserMembershipsAsync(Guid userId);
        Task<List<Member>> GetGroupMembersAsync(Guid groupId);
        void DeleteMember(Member member);

        Task<bool> IsUserAMemberOfAGroup(Guid userId, Guid groupId);

        void CreateOpinion(Opinion opinion);
        Task<List<Opinion>> GetOpinionsAsync();
        Task<List<Opinion>> GetOpinionsAboutUserAsync(string userId);
        Task<List<Opinion>> GetOpinionsOfUserAsync(string userId);
        Task<Opinion> GetOpinionAsync(Guid opinionId);
        Task<Opinion> GetOpinionNotTrackedAsync(Guid opinionId);
        void DeleteOpinion(Opinion opinion);

        void CreateReport(Report report);
        Task<List<Report>> GetIssuedReportsAsync();
        Task<List<Report>> GetReportsAsync();
        Task<List<Report>> GetUserReportsAsync(Guid userId);
        Task<Report> GetReportAsync(Guid reportId);
        Task<Report> GetReportNotTrackedAsync(Guid reportId);
        void DeleteReport(Report report);

        void CreateTransport(Transport transport);
        Task<List<Transport>> SearchTransportsAsync(
            ICollection<TransportCategory> categories,
            Guid? startsInCityId,
            Guid? endsInCityId
        );
        Task<List<Transport>> GetUserTransportsAsync(Guid userId);
        Task<Transport> GetTransportAsync(Guid transportId);
        Task<Transport> GetTransportNotTrackedAsync(Guid transportId);
        void DeleteTransport(Transport transport);

        void CreateConversation(Conversation conversation);
        void DeleteConversation(Conversation conversation);
        Task<Conversation> GetConversation(Guid conversationId);
        Task<List<Conversation>> GetUserConversationsAsync(Guid userId);

        Task AddUserToConversation(AppUser user, Conversation conversation);
        Task RemoveUserFromConversation(AppUser user, Conversation conversation);

        Task<List<Message>> GetMessagesFromConversation(Guid conversationId);
        Task<List<Message>> GetLastMessagesFromConversation(Guid conversationId, int count);

        Task<int> CountOfNewDemands(DateTime before);
        Task<int> CountOfNewOpinions(DateTime before);
        Task<int> CountOfNewTransports(DateTime before);
        Task<int> CountOfNewGroups(DateTime before);
    }
}