create procedure TEST_Benkyou_users.dbo.createUser(
    @id as uniqueidentifier,
    @user_name as nvarchar(16),
    @first_name as nvarchar(20),
    @last_name as nvarchar(35)) as
begin
    INSERT INTO TEST_Benkyou_users.dbo.UsersInformation(Id, UserName, FirstName, LastName, IsTermsAccepted, IsAccountPublic)
    VALUES (@id, @user_name, @first_name, @last_name, 1, 0)
end;


    create procedure TEST_Benkyou_users.dbo.getUserById(@user_id AS uniqueidentifier)
    as
    begin
        SELECT *
        FROM TEST_Benkyou_users.dbo.UsersInformation
        WHERE Id = @user_id
    end;

        create procedure TEST_Benkyou_users.dbo.updateUser(
            @user_id as uniqueidentifier,
            @first_name as nvarchar(20),
            @last_name as nvarchar(35),
            @user_name as nvarchar(16),
            @birth_day as datetime2,
            @is_account_public as bit,
            @about as nvarchar(350)
        ) as
        begin
            update TEST_Benkyou_users.dbo.UsersInformation
            set FirstName       = @first_name,
                LastName        = @last_name,
                UserName        = @user_name,
                BirthDay        = @birth_day,
                IsAccountPublic = @is_account_public,
                About           = @about
            where Id = @user_id
        end;

            create procedure TEST_Benkyou_users.dbo.updateUserAvatar(
                @user_id as uniqueidentifier,
                @avatar_url as nvarchar(100)
            ) as
            begin
                update TEST_Benkyou_users.dbo.UsersInformation
                set AvatarUrl = @avatar_url
                where Id = @user_id
            end;