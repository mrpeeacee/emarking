export class AnnotationSettings{
    IsAnnotationsMandatory!:boolean;
    IsTagged!:boolean;
    IsScriptTrialMarked!:boolean;
  }
  


  export class QigAnnotationDetails {
    TemplateName!: string;
    TemplateCode!: string; 
    AnnotationGroup!: AnnotationGroups[]; 
}

export class AnnotationGroups {
   GroupName!: string;
   GroupCode!: string; 
   AnnotationTools!: AnnotationTools[]; 
}

export class AnnotationTools {
   AnnotationToolName!:string;
   AnnotationToolCode!:string;
   Path!: string;
   ColorCode!: string;  
   isChecked!: boolean;
}