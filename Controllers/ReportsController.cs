using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DowiezPlBackend.Data;
using DowiezPlBackend.Dtos;
using DowiezPlBackend.Dtos.Account;
using DowiezPlBackend.Dtos.Report;
using DowiezPlBackend.Enums;
using DowiezPlBackend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DowiezPlBackend.Controllers
{
    public class ReportsController : DowiezPlControllerBase
    {
        IDowiezPlRepository _repository;
        IMapper _mapper;
        
        public ReportsController(IDowiezPlRepository repository, IMapper mapper, UserManager<AppUser> userManager) : base(userManager)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns all reports
        /// </summary>
        /// <response code="200">Returns an array of reports</response>
        [HttpGet]
        [Authorize(Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReportSimpleReadDto>>> GetReports(
            ReportCategory? category,
            ReportStatus? status,
            bool? isAssigned,
            bool? assignedToMe
        )
        {
            var me = await GetMyUserAsync();
            var results = await _repository.GetReportsFilterAsync(
                category,
                status,
                isAssigned == null ? null : (bool)isAssigned,
                assignedToMe == null ? null : ((bool)assignedToMe ? me : null));
            return Ok(_mapper.Map<IEnumerable<ReportSimpleReadDto>>(results));
        }
        
        /// <summary>
        /// Returns reports created by currently logged user
        /// </summary>
        /// <response code="200">Returns an array of reports</response>
        [HttpGet("my")]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReportSimpleReadDto>>> GetMyReports()
        {
            var userDb = await GetMyUserAsync();
            var results = await _repository.GetUserReportsAsync(userDb.Id);
            return Ok(_mapper.Map<IEnumerable<ReportSimpleReadDto>>(results));
        }

        /// <summary>
        /// Returns report
        /// </summary>
        /// <param name="reportId">Report's Id</param>
        /// <response code="200">Returns a report</response>
        /// <response code="403">Only creator or moderator can see this report</response>
        /// <response code="404">Report not found</response>
        [HttpGet("{reportId}", Name = "GetReport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ReportReadDto>> GetReport(Guid reportId)
        {
            var reportFromRepo = await _repository.GetReportNotTrackedAsync(reportId);
            if (reportFromRepo == null)
                return NotFound();
            
            var userDb = await GetMyUserAsync();

            if (!await IsModerator(userDb)
                && reportFromRepo.Reporter.Id != userDb.Id)
            {
                return Forbid();
            }

            var dto = _mapper.Map<ReportReadDto>(reportFromRepo);
            dto.ReporterDto = _mapper.Map<AccountLimitedReadDto>(reportFromRepo.Reporter);
            
            if (reportFromRepo.Reported != null)
                dto.ReportedDto = _mapper.Map<AccountLimitedReadDto>(reportFromRepo.Reported);
            if (reportFromRepo.Operator != null)
                dto.OperatorDto = _mapper.Map<AccountLimitedReadDto>(reportFromRepo.Operator);


            return Ok(dto);
        }

        /// <summary>
        /// Creates a report
        /// </summary>
        /// <param name="reportCreateDto">New report's data</param>
        /// <response code="201">Report was created successfully</response>
        /// <response code="400">Creation of a report failed</response>
        /// <response code="404">Some not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ReportReadDto>> CreateReport(ReportCreateDto reportCreateDto)
        {
            var issuer = await GetMyUserAsync();
            AppUser reported = null;
            Transport reportedTransport = null;
            Demand reportedDemand = null;
            Group reportedGroup = null;

            if (reportCreateDto.ReportedId != null)
            {
                var user = await GetUserAsync(reportCreateDto.ReportedId.ToString());
                if (user == null)
                    return BadRequest(new ErrorMessage("Failed to create a report.", "RC_CR_1"));
                if (await IsModerator(user))
                    return BadRequest(new ErrorMessage("Failed to create a report.", "RC_CR_2"));
                reported = user;
            }

            if (reportCreateDto.ReportedTransportId != null)
            {
                reportedTransport = await _repository.GetTransportAsync((Guid)reportCreateDto.ReportedTransportId);
                if (reportedTransport == null)
                    return NotFound(new ErrorMessage("Transport not found.", "RC_CR_3"));
            }

            if (reportCreateDto.ReportedDemandId != null)
            {
                reportedDemand = await _repository.GetDemandAsync((Guid)reportCreateDto.ReportedDemandId);
                if (reportedDemand == null)
                    return NotFound(new ErrorMessage("Demand not found.", "RC_CR_4"));
            }

            if (reportCreateDto.ReportedGroupId != null)
            {
                reportedGroup = await _repository.GetGroupAsync((Guid)reportCreateDto.ReportedGroupId);
                if (reportedGroup == null)
                    return NotFound(new ErrorMessage("Demand not found.", "RC_CR_5"));
            }

            var report = _mapper.Map<Report>(reportCreateDto);
            report.Reporter = issuer;
            report.Reported = reported;
            report.CreationDate = DateTime.UtcNow;
            report.Status = ReportStatus.Issued;

            report.ReportedTransport = reportedTransport;
            report.ReportedDemand = reportedDemand;
            report.ReportedGroup = reportedGroup;

            _repository.CreateReport(report);
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to create a report.", "RC_CR_6"));
            
            var reportReadDto = _mapper.Map<ReportReadDto>(report);
            return CreatedAtRoute(nameof(GetReport), new { reportId = reportReadDto.ReportId }, reportReadDto);
        }

        /// <summary>
        /// Cancels a report
        /// </summary>
        /// <param name="reportId">Report's Id</param>
        /// <response code="204">Report was canceled successfully</response>
        /// <response code="400">Cancelation failed</response>
        /// <response code="403">Only creator can cancel this report</response>
        /// <response code="404">Report not found</response>
        [HttpPut("cancel/{reportId}")]
        [Authorize(Roles = "Standard")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CancelReport(Guid reportId)
        {
            var reportFromRepo = await _repository.GetReportAsync(reportId);
            if (reportFromRepo == null)
                return NotFound();
            
            if (reportFromRepo.Reporter.Id != (await GetMyUserAsync()).Id)
                return Forbid();
            
            if (reportFromRepo.Status != ReportStatus.Issued)
                return BadRequest(new ErrorMessage($"Cannot cancel report with status {reportFromRepo.Status}.", "RC_CaR_2"));
            
            reportFromRepo.Status = ReportStatus.Canceled;
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to cancel a report.", "RC_CaR_1"));
            
            return NoContent();
        }

        /// <summary>
        /// Changes the status of a report
        /// </summary>
        /// <param name="reportId">Report's Id</param>
        /// <param name="status">Report's new status</param>
        /// <response code="204">Report was updated successfully</response>
        /// <response code="400">Status change failed</response>
        /// <response code="404">Report not found</response>
        [HttpPut("status/{reportId}/{status}")]
        [Authorize(Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateReportStatus(Guid reportId, ReportStatus status)
        {
            var reportFromRepo = await _repository.GetReportAsync(reportId);
            if (reportFromRepo == null)
                return NotFound();
            
            if (reportFromRepo.Status == ReportStatus.Issued)
                return BadRequest(new ErrorMessage($"Invalid operation. Tried to change {ReportStatus.Issued} to {status}. The {ReportStatus.Issued} status can only be change using {nameof(AssignReport)} method.", "RC_URS_2"));
            
            if (status == ReportStatus.Issued)
                return BadRequest(new ErrorMessage($"Invalid operation. Tried to change {reportFromRepo.Status} to {ReportStatus.Issued}.", "RC_URS_3"));
            
            if (reportFromRepo.Status == ReportStatus.Canceled)
                return BadRequest(new ErrorMessage($"Invalid operation. Cannot interact with report with status {ReportStatus.Canceled}", "RC_URS_4"));
            
            reportFromRepo.Status = status;
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to update a report.", "RC_URS_1"));
            
            return NoContent();
        }

        /// <summary>
        /// Assigns report to moderator and changes the status of a report to InProgess
        /// </summary>
        /// <param name="reportId">Report's Id</param>
        /// <response code="204">Report was updated successfully</response>
        /// <response code="400">Assignation failed</response>
        /// <response code="404">Report not found</response>
        [HttpPut("assign/{reportId}")]
        [Authorize(Roles = "Moderator,Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorMessage), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AssignReport(Guid reportId)
        {
            var reportFromRepo = await _repository.GetReportAsync(reportId);
            if (reportFromRepo == null)
                return NotFound();
            
            if (reportFromRepo.Operator != null)
                return BadRequest(new ErrorMessage("Cannot assign a report that is already assigned to someone.", "RC_AR_1"));
            
            reportFromRepo.Operator = await GetMyUserAsync();
            reportFromRepo.Status = ReportStatus.InProgress;
            if (!await _repository.SaveChangesAsync())
                return BadRequest(new ErrorMessage("Failed to update a report.", "RC_AR_2"));
            
            return NoContent();
        }
    }
}