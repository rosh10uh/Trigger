SELECT Id, email, SecurityStamp FROM aspnetusers WHERE email = @email
AND EmailConfirmed = 1
