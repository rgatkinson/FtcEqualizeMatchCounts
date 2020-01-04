# FtcEqualizeMatchCounts

## Summary
``FtcEqualizeMatchCounts`` is a tool for editing the database used by the [First Tech Challenge ScoreKeeper software](https://github.com/FIRST-Tech-Challenge/scorekeeper) so as that at the start of a League Tournament all teams have the same number of Averaging Matches accrued from prior League Meets. The tool does this by adding never-played Equalization Matches to the event schedule as necessary. After importign the edited database back into ScoreKeeper, these Equalization Matches need to be manually scored as Wins for Blue.

## Installation
``FtcEqualizeMatchCounts`` is a Windows-only command line tool. To install, run the ``Setup`` program found in the release. Once installed, run the 'FTC Equalize Match Counts Command Prompt' item from the Start Menu to open a command prompt window with the tool in scope. Running ``FtcEqualizeMatchCounts.exe -?`` will then print a brief usage message.

## Work FLow
To use the tool proceed as follows.
* Set up an event in ScoreKeeper as usual to (at least) the point of Creating the Match Schedule.
*	Export the event database using ‘Download Archive File’ on the Event Dashboard
*	Run ``FTCEqualizeMatchCounts event.db`` to edit the database.
    - The default options are almost always sufficient
    - The default is to bring all teams up to the same Averaging Match count; this may be ten or less than ten, depending on league history. Use ``-c 10`` if exactly ten averaging matches is desired.
    - Note: a backup of the unmodified database file is made before changes are made
*	Return to ScoreKeeper and ‘hide’ the event using ‘Event Hiding’ on the ‘Event Admin / Server’ page.
*	Import the now-modified database using ‘Data Import’ on that same page
*	(Optional): Observe the added Equalization Matches on the Match Schedule
*	Finish setting up the event if needed
*	On the Match Control Page use ‘Enter Scores’ to score each Equalization Match as a win for Blue

