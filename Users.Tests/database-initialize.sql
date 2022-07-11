create
database TEST_Benkyou_users
go
use TEST_Benkyou_users
go
create table UsersInformation
(
    Id              uniqueidentifier default newid() not null
        constraint UsersInformation_pk
            primary key,
    FirstName       nvarchar(20) not null,
    LastName        nvarchar(35) not null,
    UserRole        nvarchar(10) default N'User' not null,
    BirthDay        datetime2,
    AvatarUrl       nvarchar(100),
    IsTermsAccepted bit                              not null,
    IsAccountPublic bit                              not null,
    About           nvarchar(350),
    UserName        nvarchar(16) not null
)
    go


create unique index UsersInformation_Id_uindex
    on UsersInformation (Id);