﻿using System;

#pragma warning disable 649
namespace FEMC.DBTables
    {
    class QualsScoresHistory : Table<QualsScoresHistory.Row, Tuple<long, DateTimeAsInteger>>
        {
        public class Row : TableRow<Row, Tuple<long, DateTimeAsInteger>>
            {
            public NullableLong MatchNumber;
            public DateTimeAsInteger Ts;

            public NullableLong Alliance;
            public NullableLong Card1;
            public NullableLong Card2;
            public NullableLong DQ1;
            public NullableLong DQ2;
            public NullableLong NoShow1;
            public NullableLong NoShow2;
            public NullableLong Major;
            public NullableLong Minor;
            public NullableLong Adjust;

            public override Tuple<long, DateTimeAsInteger> PrimaryKey => new Tuple<long, DateTimeAsInteger>(MatchNumber.NonNullValue, Ts);
            }

        public override string TableName => "qualsScoresHistory";

        public QualsScoresHistory(Database database) : base(database)
            {
            }
        }
    }