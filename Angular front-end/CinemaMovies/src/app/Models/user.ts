export class Users{

  UserName:string='';
  NormalizedUserName :string='';
  Email :string='';
  NormalizedEmail :string='';
  EmailConfirmed :string='';
  PasswordHash :string='';
  SecurityStamp :string='';
  ConcurrencyStamp :string='';
  PhoneNumber :string='';
  PhoneNumberConfirmed :string='';
  TwoFactorEnabled :boolean=false;
  LockoutEnd :Date=new Date();
  LockoutEnabled :boolean=false;
  AccessFailedCount :number=0
  Country :string='';


}
