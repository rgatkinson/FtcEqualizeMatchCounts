using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Permissions;
using FEMC.DBTables;
using FEMC.Enums;

namespace FEMC.DAL
    {
    class ScheduledMatch : ThisEventMatch
        {
        //----------------------------------------------------------------------------------------
        // State
        //----------------------------------------------------------------------------------------

        protected long matchNumber;
        public string Description;
        public string CreatedBy;
        protected int tournamentLevel;
        protected int fieldType;
        public DateTimeOffset ScheduleStart;
        protected Guid? fmsMatchIdGuid;
        public void SetMatchType(TMatchType value) => tournamentLevel = value == TMatchType.QUALS ? 2 : (value == TMatchType.ELIMS ? 3 : 0);

        public TFieldType FieldType => Enum.IsDefined(typeof(TFieldType), fieldType) ? (TFieldType)fieldType : TFieldType.Unknown;
        public Guid FMSMatchIdGuid 
            {
            get
                {
                if (fmsMatchIdGuid != null)
                    return fmsMatchIdGuid.Value;
                else
                    {
                    var row = Database.Tables.QualsData.FindFirstRow(r => Equals(r.FMSScheduleDetailId.NonNullValue, FMSScheduleDetailId.NonNullValue));
                    Trace.Assert(row != null);
                    return row.FMSMatchId.NonNullValue;
                    }
                }
            }

        public Team Red1;
        public bool Red1Surrogate;
        public Team Red2;
        public bool Red2Surrogate;
        public Team Blue1;
        public bool Blue1Surrogate;
        public Team Blue2;
        public bool Blue2Surrogate;

        //----------------------------------------------------------------------------------------
        // Overrides
        //----------------------------------------------------------------------------------------

        public override ScheduledMatch Scheduled => this;

        public override FMSEventId FMSEventId => fmsEventId?.Value == null ? Database.ThisFMSEventId : fmsEventId;

        public override long MatchNumber => matchNumber;

        public override bool IsEqualizationMatch => Equals(CreatedBy, Database.EqualizationMatchCreatorName) && MatchNumber >= Database.FirstEqualizationMatchNumber;

        public override TMatchType MatchType // see SQLiteManagementDAO.java/saveFMSSchedule
            {
            get
                {
                if (tournamentLevel == 2) return TMatchType.QUALS;
                if (tournamentLevel == 3) return TMatchType.ELIMS;
                return TMatchType.TEST; // should never happen?
                }
            }

        //----------------------------------------------------------------------------------------
        // Accessing
        //----------------------------------------------------------------------------------------

        public override ICollection<int> PlayedTeams
            {
            get {
                List<int> result = new List<int>();
                if (!Red1Surrogate) result.Add(Red1.TeamNumber);
                if (!Red2Surrogate) result.Add(Red2.TeamNumber);
                if (!Blue1Surrogate) result.Add(Blue1.TeamNumber);
                if (!Blue2Surrogate) result.Add(Blue2.TeamNumber);
                return result;
                }
            }

        public Team GetTeam(TAlliance alliance, TStation station)
            {
            if (alliance == TAlliance.Red)
                {
                return station==TStation.Station1 ? Red1 : Red2;
                }
            else
                {
                return station == TStation.Station1 ? Blue1 : Blue2;
                }
            }

        public bool GetSurrogate(TAlliance alliance, TStation station)
            {
            if (alliance == TAlliance.Red)
                {
                return station == TStation.Station1 ? Red1Surrogate : Red2Surrogate;
                }
            else
                {
                return station == TStation.Station1 ? Blue1Surrogate : Blue2Surrogate;
                }
            }

        //----------------------------------------------------------------------------------------
        // Construction
        //----------------------------------------------------------------------------------------

        protected static FMSScheduleDetailId NewFMSScheduleDetailId()
            {
            FMSScheduleDetailId result = new FMSScheduleDetailId();
            result.Value = Guid.NewGuid();
            return result;
            }

        protected void AddToDatabase()
            {
            Trace.Assert(!Database.ScheduledMatchesByNumber.ContainsKey(MatchNumber));
            Trace.Assert(!Database.ScheduledMatchesById.ContainsKey(FMSScheduleDetailId));

            Database.ScheduledMatchesByNumber[MatchNumber] = this;
            Database.ScheduledMatchesById[FMSScheduleDetailId] = this;
            Database.ThisEvent.AddMatch(this);
            }

        protected ScheduledMatch(Database db, FMSEventId eventId, FMSScheduleDetailId detailId) : base(db, eventId, detailId)
            {
            }

        public ScheduledMatch(Database db, DBTables.ScheduleDetail.Row row) : base(db, row.FMSEventId, row.FMSScheduleDetailId)
            {
            FMSScheduleDetailId = row.FMSScheduleDetailId;
            matchNumber = row.MatchNumber.NonNullValue;
            Description = row.Description.Value;
            CreatedBy = row.CreatedBy.Value;
            tournamentLevel = (int)row.TournamentLevel.NonNullValue;
            fieldType = (int)row.FieldType.NonNullValue;
            ScheduleStart = row.StartTime.DateTimeOffsetNonNull;

            var qual = db.Tables.Quals.Map[row.MatchNumber.NonNullValue];

            Red1 = db.TeamsByNumber[qual.Red1.NonNullValue];
            Red2 = db.TeamsByNumber[qual.Red2.NonNullValue];
            Blue1 = db.TeamsByNumber[qual.Blue1.NonNullValue];
            Blue2 = db.TeamsByNumber[qual.Blue2.NonNullValue];

            Red1Surrogate = qual.Red1Surrogate.NonNullValue;
            Red2Surrogate = qual.Red2Surrogate.NonNullValue;
            Blue1Surrogate = qual.Blue1Surrogate.NonNullValue;
            Blue2Surrogate = qual.Blue2Surrogate.NonNullValue;

            AddToDatabase();
            }
        }
    }
