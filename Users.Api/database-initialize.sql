create database Benkyou_users
go

use Benkyou_users
go

create table UsersInformation
(
    Id              uniqueidentifier default newid() not null
        constraint UsersInformation_pk
            primary key,
    FirstName       nvarchar(20)                     not null,
    LastName        nvarchar(35)                     not null,
    UserRole        nvarchar(10)     default N'User' not null,
    BirthDay        datetime2,
    AvatarUrl       nvarchar(100),
    IsTermsAccepted bit                              not null,
    IsAccountPublic bit                              not null,
    About           nvarchar(350),
    UserName        nvarchar(16)                     not null
)
go

create unique index UsersInformation_Id_uindex
    on UsersInformation (Id)
go

use Benkyou_users
go

create procedure createUser(
@id as uniqueidentifier,
@user_name as nvarchar(16),
@first_name as nvarchar(20),
@last_name as nvarchar(35)) as
    begin
        INSERT INTO UsersInformation(Id, UserName, FirstName, LastName, IsTermsAccepted, IsAccountPublic)
        VALUES
            (@id, @user_name, @first_name, @last_name, 1, 0)
    end
go

create procedure getUserById(@user_id AS uniqueidentifier)
as
    begin
        SELECT * FROM UsersInformation WHERE Id = @user_id
    end
go

create procedure updateUser(
@user_id as uniqueidentifier,
@first_name as nvarchar(20),
@last_name as nvarchar(35),
@user_name as nvarchar(16),
@birth_day as datetime2,
@is_account_public as bit,
@about as nvarchar(350)
) as
    begin
        update UsersInformation
        set
            FirstName = @first_name,
            LastName = @last_name,
            UserName = @user_name,
            BirthDay = @birth_day,
            IsAccountPublic = @is_account_public,
            About = @about
        where Id = @user_id
    end
go

create procedure updateUserAvatar(
@user_id as uniqueidentifier,
@avatar_url as nvarchar(100)
) as
    begin
        update UsersInformation
        set AvatarUrl = @avatar_url
        where Id = @user_id
    end
go



