import { AppSettingEntityType, AppSettingValueType } from "src/app/model/appsetting/app-setting-model";

export class KeyPersonnelsData {
  UserID!: number;
  ProjectUserRoleID!: number;
  LoginName!: string;
  RoleID!: number;
  RoleCode!: string;
  IsKP?: boolean;
  IsKpTagged?: boolean;
  IsKpTrialmarkedorcategorised?: boolean;
}

export class ProjectCenteres {
  ProjectCenterID!: number;
  IsRecommended!: boolean;
  IsrecDisabled!: boolean;
  ProjectID!: number;
  CenterID!: number;
  CenterName!: string;
  CenterCode!: string;
  TotalNoOfScripts!: number;
  IsSelectedForRecommendation!: boolean;
  checked!: boolean;
  Children: ProjectCenteres[] = [];
  noresponsecount!: number;
}

export class QigConfigurationData {
  QIGID!: number;
  QIGCode!: string;
  QIGName!: string;
  RecomendationPoolCount!: number;
  RecomendationPoolCountPerKP!: number;
  script_total!: number;
  AppSettingKeyIDPoolCount!: number;
  AppSettingKeyIDPoolCountPerKP!: number;
  Ispauseoronholdors1comp!: boolean;
  RecommendationPoolCountAppSettingKey!: string;
  RecommendationCountKPAppSettingKey!: string;
}

export class GetAppSettingGroupModel {
  SettingGroupID!: number
  SettingGroupCode!: string
  SettingGroupName!: string
}

export class UpdateProjectConfigModel {
  SettingGroupID!: number
  SettingGroupCode!: string
  SettingGroupName!: string
  AppsettingKey!: string
  AppsettingKeyName!: string
  AppSettingGroupId!: number
  ParentAppsettingKeyID!: number
  EntityID!: number
  EntityType!: AppSettingEntityType
  AppSettingKeyID!: number
  Value!: any
  DefaultValue !: string
  ValueType!: AppSettingValueType
  ReferanceID !: number
  ProjectID!: number
  Children: UpdateProjectConfigModel[] = []
}
