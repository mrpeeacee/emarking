export interface ScoreComponentLibraryName {
 
  ScoreComponentId:number;
  ComponentName: string;
   ComponentCode: string;
 
   Marks?: number;
   ProjectID: number;
   IsTagged: Boolean;
   ScoringComponentDetails: ScoringComponentDetails[];
   IsActive: boolean;
   IsDeleted: boolean;
   
   CreatedBy: number;
  //  IsBandExist: boolean;
   IsQuestionTagged: boolean;

  
 }
 
 export interface ScoringComponentDetails{
  ComponentDetailID:number
  Order:number
  ComponentCode:any
  ScoringComponentName:any;
  Marks?:number;
  IsQuestionTagged?:boolean;
 }