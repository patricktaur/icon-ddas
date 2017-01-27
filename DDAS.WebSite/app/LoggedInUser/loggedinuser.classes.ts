    export class Role
    {
        Name : string;
        RoleId: string;
        Selected: boolean;
    }
    
    export class User{
        UserId: string;
        UserName : string;
        Roles: Role[];
        EmailId : string;
        UserFullName : string;
        RoleName : string;
        pwd:string;
    }

    export class UserViewModel
    {
         UserId : string;
         UserName : string;
          EmailId : string;
          UserFullName : string;
         Active: Boolean;
         Roles: RoleViewModel[];
        ActiveRoles : string;
    }

    export class RoleViewModel
    {
        Active : Boolean;
        Name : string;
    }