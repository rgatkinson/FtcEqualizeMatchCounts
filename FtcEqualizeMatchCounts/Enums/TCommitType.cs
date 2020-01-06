namespace FEMC.Enums
    {
    enum TCommitType // org.usfirst.ftc.event.CommitType
        {
        [StringValue("Commit")] COMMIT = 0,
        [StringValue("Match Timer End")] MATCH_END = 1,
        [StringValue("Red Review")] RED_REF_REVIEW = 2,
        [StringValue("Blue Review")] BLUE_REF_REVIEW = 3,
        [StringValue("Red Submit")] RED_REF_SUBMIT = 4,
        [StringValue("BlueSubmit")] BLUE_REF_SUBMIT = 5,
        [StringValue("Scorekeeper Edit")] EDIT_SAVED = 6,
        }
    }