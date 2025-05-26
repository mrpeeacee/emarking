export enum Userrole {
  EO = "EO",
  AO = "AO",
  CM = "CM",
  ACM = "ACM",
  TL = "TL",
  ATL = "ATL",
  MARKER = "MARKER",
  All = "All",
  KP = "KP",
  EM = "EM",
  SUPERADMIN = "SUPERADMIN",
  SERVICEADMIN = "SERVICEADMIN"
}

export class MenuItem {
  _id!: string;
  name!: string;
  path!: string;
  sub!: MenuItem[];
}


