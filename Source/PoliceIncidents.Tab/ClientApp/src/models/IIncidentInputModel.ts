export interface IIncidentInputModel {
  title: string;
  description: string;
  managerId: string;
  location: string;
  regionId: string;
  groupIds: string[];
  memberIds: string[];
}
