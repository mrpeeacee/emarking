

export class StandardisationPracticeAssessmentModel{
    Qigname!: string;
    Noofscripts!: number;
    Markedscript!: number;
    Result!: number;
    TotalMarks!: number;
    ProcessStatus!:number;
    Isopened: boolean = false;
    Scripts!: StandardisationScriptClass[];
}

export class StandardisationScriptClass{
    ScriptName!: string;
    IsCompleted!: boolean;
    Result!: number;
    MarkedBy!: number;
    ScriptId!: number;
    WorkflowStatusID!: number;
    UserMarkedBy!: number;
}

export class PracticeQuestionDetails{
    QuestionLabel!: string;
    TotalMarks!: number;
    DefenetiveMarks!: number;
    AwardedMarks!: number;
    ToleranceLimit!: number;
    IsOutOfTolerance!: boolean;
}