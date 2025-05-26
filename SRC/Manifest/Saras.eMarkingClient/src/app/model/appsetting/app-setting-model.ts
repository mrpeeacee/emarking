export enum AppSettingGroup {
    ProjectGroupCode = "PRJCTSTTNG",
    AnnotationGroupCode = "ANNTSN",
    Standardization1SettingGroupCode= 'STTNGS1',
}
export enum AppSettingEntityType {
    None = 0,
    Project = 1,
    QIG = 2,
    User = 3,
    Role = 4,
    Question = 5
}

export enum AppSettingValueType {
    String = 1,
    Integer = 2,
    Float = 3,
    XML = 4,
    DateTime = 5,
    Bit = 6,
    Int = 7,
    BigInt = 8,
    None = 0
}

export class AppSettingModel {
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
    Children: AppSettingModel[] = []
}
