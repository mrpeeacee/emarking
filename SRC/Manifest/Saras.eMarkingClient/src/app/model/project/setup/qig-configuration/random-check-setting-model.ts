import { AppSettingModel } from 'src/app/model/appsetting/app-setting-model';

export class QigModel {
    QigId!: number;
    QigName!: string;
    ShowTeam!: boolean;
    ShowAnnotation!: boolean;
    ShowRc!: boolean;
    ShowQns!: boolean;
    TeamIds!: ProjectTeamsIdsModel[];
    RandomCheckSettings!: AppSettingModel[];
    RcType!: RandomCheckType;
    AnnotationSetting: AppSettingModel[] = []
}

export class ProjectTeamsIdsModel {
    QigId!: number;
    TeamQigId!: number;
    TeamId!: number;
    IsChecked!: boolean;
}


export enum RandomCheckType {
    OneTier = 1,
    TwoTier = 2,
    None = 0
}