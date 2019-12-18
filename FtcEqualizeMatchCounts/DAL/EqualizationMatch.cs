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
            DBTables.ScheduledMatch.Row scheduledMatchRow = new DBTables.ScheduledMatch.Row();
            DBTables.Quals.Row qualRow = new Quals.Row();

            scheduledMatchRow.InitializeFields();
            qualRow.InitializeFields();

            scheduledMatchRow.FMSScheduleDetailId = FMSScheduleDetailId;
            scheduledMatchRow.FMSEventId = FMSEventId;
            scheduledMatchRow.TournamentLevel.Value = 2; // 'qualification match'
            scheduledMatchRow.MatchNumber.Value = MatchNumber;
            scheduledMatchRow.FieldType.Value = 1; // todo: is this right? haven't ever seen any other values
            scheduledMatchRow.Description.Value = Description;
            scheduledMatchRow.StartTime.Value = DateTimeOffset.Now;
            scheduledMatchRow.FieldConfigurationDetails.Value = null;
            scheduledMatchRow.CreatedOn.Value = null;
            scheduledMatchRow.CreatedBy.Value = CreatedBy;
            scheduledMatchRow.ModifiedOn.Value = null;
            scheduledMatchRow.ModifiedBy.Value = null;
            scheduledMatchRow.RowVersion.Value = Guid.Empty; // ScheduleDetail seems to use 16 byte all-zero RowVersions; 'don't know why

            qualRow.Match.Value = MatchNumber;
            qualRow.Red1.Value = Red1.TeamNumber;
            qualRow.Red2.Value = Red2.TeamNumber;
            qualRow.Blue1.Value = Blue1.TeamNumber;
            qualRow.Blue2.Value = Blue2.TeamNumber;
            qualRow.Red1Surrogate.Value = Red1Surrogate;
            qualRow.Red2Surrogate.Value = Red2Surrogate;
            qualRow.Blue1Surrogate.Value = Blue1Surrogate;
            qualRow.Blue2Surrogate.Value = Blue2Surrogate;

            Database.Tables.ScheduledMatch.AddRow(scheduledMatchRow);
            Database.Tables.Quals.AddRow(qualRow);

            scheduledMatchRow.SaveToDatabase();
            qualRow.SaveToDatabase();

            foreach (var alliance in EnumUtil.GetValues<Alliance>())
                {
                foreach (var station in EnumUtil.GetValues<Station>())
                    {
                    DBTables.ScheduledMatchStation.Row row = new DBTables.ScheduledMatchStation.Row();
                    row.InitializeFields();

                    row.FMSScheduleDetailId = FMSScheduleDetailId;
                    row.Alliance.Value = (int)alliance;
                    row.Station.Value = (int)station;

                    row.FmsEventId = FMSEventId;
                    row.FmsTeamId = GetTeam(alliance, station).FMSTeamId;
                    row.IsSurrogate.Value = GetSurrogate(alliance, station) ? 1 : 0;

                    row.CreatedOn = scheduledMatchRow.CreatedOn;
                    row.CreatedBy = scheduledMatchRow.CreatedBy;
                    row.ModifedOn = scheduledMatchRow.ModifiedOn;
                    row.ModifiedBy = scheduledMatchRow.ModifiedBy;

                    Database.Tables.ScheduledMatchStation.AddRow(row);
                    row.SaveToDatabase();
                    }
                }
            }
        }
    }
