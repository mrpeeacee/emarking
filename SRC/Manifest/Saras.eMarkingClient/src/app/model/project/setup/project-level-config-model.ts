import { AppSettingEntityType, AppSettingValueType } from "../../appsetting/app-setting-model"

export class ProjectLevelConfigModel {
    SettingGroupID!: number
    SettingGroupCode!: string
    SettingGroupName!: string
    AppsettingKey!: string
    AppsettingKeyName!: string
    AppSettingGroupId!:number
    ParentAppsettingKeyID!: number
    EntityID!: number
    EntityType!: AppSettingEntityType
    AppSettingKeyID!: number
    Value!: any
    DefaultValue !: string
    ValueType!: AppSettingValueType
    ReferanceID !: number
    ProjectID!: number
    ProjectStatus!: number
    Children: ProjectLevelConfigModel[] = []
}

export class RandomCheckModel{
    Tier1TimeDuration!: number;
    Tier2TimeDuration!: number;
}
