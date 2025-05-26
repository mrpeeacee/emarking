export class Globalconst {
    public static projectId: number;
    public static infoText: string; 
}

export class BrandingModel {
    public DisplayBuildNumber!: boolean;
    public BuildNumber!: string;
    public Branding!: Branding;
}
export class Branding {
    public LogoPath!: string;
    public Copyright!: string;
    public Year!: string;
    public DefaultUserImage!: string;
}