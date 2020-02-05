SELECT 'Server=' + [ServerName] + ';Initial Catalog=' + [DBName] 
	+ ';Persist Security Info=False;User ID=' + [UserName] + ';Password=' + [Password]
	+ ';MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0;' as 'ConnectionString'
	FROM CompanyDbConfig WHERE [CompanyId] = @CompanyId;