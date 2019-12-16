using System;
using System.Collections.Generic;



namespace FtcEqualizeMatchCounts
    {
    // Notes:
    //      FMSMatchId is a UUID
    //          in qualsData, stored as text
    //
    //      FMSScheduleDetailId is a UUID
    //          in qualsData, stored as text
    //          in ScheduleDetail, stored as blob
    //              so is FMSEventId: is this *also* a UUID?
    //          ditto in ScheduleStation
    //              ditto FMSEventId, FMSTeamId therein

    //--------------------------------------------------------------------------------------------------------------------------
    // Base classes
    //--------------------------------------------------------------------------------------------------------------------------

    abstract class Table
        {
        protected Database database;

        protected Table(Database database)
            {
            this.database = database;
            }

        public abstract string TableName { get; }

        public List<List<object>> SelectAll()
            {
            string query = $"SELECT * FROM { TableName }";
            List<List<object>> table = database.ExecuteQuery(query);
            return table;
            }
        }


    abstract class TableRecord
        {
        //----------------------------------------------------------------------------------------------------------------------
        // Column Types
        //----------------------------------------------------------------------------------------------------------------------

        public abstract class GuidColumn
            {
            public Guid Guid;
            public bool StoreAsText = false;    // stores as a 16byte blob otherwise
            }

        public class FMSMatchId : GuidColumn
            {
            }

        public class FMSEventId : GuidColumn
            {
            }

        public class FMSRegionId : GuidColumn
            {
            }

        public class FMSScheduleDetailId : GuidColumn
            {
            }

        public class FMSTeamId : GuidColumn
            {
            }

        public class FMSHomeCMPId : GuidColumn
            {

            }

        public class RowVersion : GuidColumn // seen: size=0, size=16, but all zeros
            {

            }

        //------------------------------------

        public class StringDateTime // Nullable always?
            {
            // e.g.:
            //  2019-12-15T00:41:31.957Z
            //  2019-12-15T00:40:28.120Z

            }

        public class IntegerBool // boolean stored as 'integer' in schema instead of 'boolean'
            {

            }

        //------------------------------------

        public abstract class BlobColumn
            {

            }

        public class ScoreDetails : BlobColumn // size=348 bytes (!)
            {

            }

        public class FieldConfigurationDetails : BlobColumn
            {
            }

        public class GameSpecifics : BlobColumn
            {
            }

        }

    //--------------------------------------------------------------------------------------------------------------------------
    // Match (probably don't need: written when match is *scored*)
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_Match : Table
        {
        public class Record : TableRecord
            {
            public FMSMatchId FMSMatchId;
            public FMSScheduleDetailId FMSScheduleDetailId;
            public long PlayNumber;
            public long FieldType;
            public StringDateTime InitialPrestartTime;
            public StringDateTime FinalPreStartTime;
            public long PreStartCount;
            public StringDateTime AutoStartTime;
            public StringDateTime AutoEndTime;
            public StringDateTime TeleopStartTime;
            public StringDateTime TeleopEndTime;
            public StringDateTime RefCommitTime;
            public StringDateTime ScoreKeeperCommitTime;
            public StringDateTime PostMatchTime;
            public StringDateTime CancelMatchTime;
            public StringDateTime CycleTime;
            public long RedScore;
            public long BlueScore;
            public long RedPenalty;
            public long BluePenalty;
            public long RedAutoScore;
            public long BlueAutoScore;
            public ScoreDetails ScoreDetails;   
            public long HeadRefReview;
            public string VideoUrl;
            public StringDateTime CreatedOn;
            public string CreatedBy;     // e.g.: "Scorekeeper Commit"
            public StringDateTime ModifiedOn;
            public string ModifiedBy;
            public FMSEventId FMSEventId;
            public RowVersion RowVersion; 
            }

        public Table_Match(Database database) : base(database)
            {
            }

        public override string TableName => "Match";
        }

    //--------------------------------------------------------------------------------------------------------------------------
    // Match Schedule
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_matchSchedule : Table
        {
        public class Record : TableRecord
            {
            public DateTime     start;
            public DateTime     end;
            public long         type;
            public string       label;
            }

        public Table_matchSchedule(Database database) : base(database)
            {
            }

        public override string TableName => "matchSchedule";
        }

    //--------------------------------------------------------------------------------------------------------------------------
    // Quals
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_quals : Table
        {
        public class Record : TableRecord
            {
            public long Match;  // small integer match number: 1, 2, 3, 4, 5, ...; see ScheduleDetail.MatchNumber
            public long Red1;
            public bool Red1Surrogate;
            public long Red2;
            public bool Red2Surrogate;
            public long Blue1;
            public bool Blue1Surrogate;
            public long Blue2;
            public bool Blue2Surrogate;
            }

        public Table_quals(Database database) : base(database)
            {
            }

        public override string TableName => "quals";
        }

    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleDetail
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_ScheduleDetail : Table
        {
        public class Record : TableRecord
            {
            public FMSScheduleDetailId FMSScheduleDetailId;
            public FMSEventId FMSEventId;
            public long TournamentLevel;    // 2 for quals?
            public long MatchNumber;        // see quals.Match
            public long FieldType;          // 1 for everything we've seen
            public StringDateTime StartTime;
            public FieldConfigurationDetails FieldConfigurationDetails;
            public StringDateTime CreatedOn;    // null is ok
            public string CreatedBy;            // e.g.: "FTC Match Maker"
            public StringDateTime ModifiedOn;   // null is ok
            public string ModifiedBy;
            public RowVersion RowVersion;
            }

        public Table_ScheduleDetail(Database database) : base(database)
            {

            }

        public override string TableName => "ScheduleDetail";
        }

    //--------------------------------------------------------------------------------------------------------------------------
    // ScheduleStation
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_ScheduleStation : Table
        {
        public class Record : TableRecord // Four records for each match Alliance (1,2) x Station (1,2)
            {
            public FMSScheduleDetailId FMSScheduleDetailId;
            public long Alliance;
            public long Station;
            public FMSEventId FmsEventId;
            public FMSTeamId FmsTeamId;
            public long IsSurrogate;
            public StringDateTime CreatedOn; // can be null
            public string CreatedBy; // e.g. "FTC Match Maker"
            public StringDateTime ModifedOn;
            public string ModifiedBy;
            }


        public Table_ScheduleStation(Database database) : base(database)
            {

            }

        public override string TableName => "ScheduleStation";
        }


    //--------------------------------------------------------------------------------------------------------------------------
    // Team
    //--------------------------------------------------------------------------------------------------------------------------

    class Table_Team : Table
        {
        public class Record : TableRecord // Four records for each match Alliance (1,2) x Station (1,2)
            {
            public FMSEventId FMSEventId;
            public FMSTeamId FMSTeamId;
            public FMSRegionId FMSRegionId;
            public long TeamId; // eg: 1029074, 1028838, 1029022, etc; not sure how used?
            public long TeamNumber;
            public string TeamNameLong;
            public string TeamNameShort;
            public string RobotName;
            public string City;
            public string StateProv;
            public string Country;
            public long RookieYear;
            public IntegerBool WasAddedFromUI;
            public IntegerBool CMPPrequalified;
            public string SchoolName;
            public IntegerBool DemoTeam;
            public FMSHomeCMPId FMSHomeCMPId;
            public GameSpecifics GameSpecifics;
            public StringDateTime CreatedOn;
            public string CreatedBy; // e.g. "Team Data Download"
            public StringDateTime ModifedOn;
            public string ModifiedBy; // e.g. "Team Data Download"
            }

        public Table_Team(Database database) : base(database)
            {
            }

        public override string TableName => "Team";
        }



    }
