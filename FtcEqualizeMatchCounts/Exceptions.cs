using Microsoft.Data.Sqlite;
using System;

namespace FEMC
    {
    class CommitFailedException : SqliteException
        {
        public CommitFailedException(Database db, int rc) : base(GetMessage(db, rc), rc, rc)
            {
            SqliteException.ThrowExceptionForRC(SQLitePCL.raw.SQLITE_ABORT, db.Connection.Handle);
            }

        private static string GetMessage(Database db, int rc)
            {
            return $"SQLite Commit Failed: rc={rc}: '{SQLitePCL.raw.sqlite3_errstr(rc).utf8_to_string()}'";
            }
        }

    abstract class DatabaseNotReadyException : Exception
        {
        protected DatabaseNotReadyException(string message) : base(message)
            {
            }
        }

    class MatchScheduleNotCreatedException : DatabaseNotReadyException
        {
        public MatchScheduleNotCreatedException() : base("Please run 'Create Match Schedule' in ScoreKeeper before using this tool.")
            {
            }
        }

    class CantLoadDatabaseException : Exception
        {
        public CantLoadDatabaseException(ProgramOptions programOptions, Exception innerException=null) : base(GetMessage(programOptions), innerException)
            {
            }

        private static string GetMessage(ProgramOptions programOptions)
            {
            return $"Error loading database '{programOptions.Filename}': is this an FTC ScoreKeeper database?";
            }
        }
    }
