using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FEMC.DBTables;

namespace FEMC.DAL
    {
    class EqualizationMatch : ScheduledMatch
        {
        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        public EqualizationMatch(Database db, List<Team> teams, List<bool> isSurrogates) : base(db, db.FMSEventId, NewFMSScheduleDetailId())
            {
            CreatedBy = db.EqualizationMatchCreatorName;
            matchNumber = Event.LastMatchNumber + 1;
            Description = $"Equalization {matchNumber}";

            Red1 = teams[0];
            Red2 = teams[1];
            Blue1 = teams[2];
            Blue2 = teams[3];

            Red1Surrogate = isSurrogates[0];
            Red2Surrogate = isSurrogates[1];
            Blue1Surrogate = isSurrogates[2];
            Blue2Surrogate = isSurrogates[3];

            AddToDatabase();
            }

        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public void SaveToDatabase()
            {
            DBTables.ScheduledMatch.Row detailRow = new DBTables.ScheduledMatch.Row();
            DBTables.Quals.Row qualRow = new Quals.Row();

            detailRow.InitializeFields();
            qualRow.InitializeFields();

            detailRow.FMSScheduleDetailId = FMSScheduleDetailId;
            detailRow.FMSEventId = Database.FMSEventId;
            detailRow.TournamentLevel.Value = 2; // 'qualification match'
            detailRow.MatchNumber.Value = MatchNumber;
            detailRow.FieldType.Value = 1; // todo: is this right? haven't ever seen any other values
            detailRow.Description.Value = Description;
            detailRow.StartTime.Value = DateTimeOffset.Now;
            detailRow.FieldConfigurationDetails.Value = null;
            detailRow.CreatedOn.Value = null;
            detailRow.CreatedBy.Value = CreatedBy;
            detailRow.ModifiedOn.Value = null;
            detailRow.ModifiedBy.Value = null;
            detailRow.RowVersion.Value = Guid.NewGuid();

            qualRow.Match.Value = MatchNumber;
            qualRow.Red1.Value = Red1.TeamNumber;
            qualRow.Red2.Value = Red2.TeamNumber;
            qualRow.Blue1.Value = Blue1.TeamNumber;
            qualRow.Blue2.Value = Blue2.TeamNumber;
            qualRow.Red1Surrogate.Value = Red1Surrogate;
            qualRow.Red2Surrogate.Value = Red2Surrogate;
            qualRow.Blue1Surrogate.Value = Blue1Surrogate;
            qualRow.Blue2Surrogate.Value = Blue2Surrogate;
            }
        }
    }
