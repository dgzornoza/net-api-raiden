﻿
using $ext_safeprojectname$.Domain.SeedWork;

namespace $safeprojectname$.Services.Audit;

public class AuditTypeDto : Enumeration
{
    public static readonly AuditTypeDto Request = new(1, "Request");
    public static readonly AuditTypeDto Response = new(2, "Response");

    private AuditTypeDto()
    {
    }

    private AuditTypeDto(int value, string displayName)
        : base(value, displayName)
    {
    }
}
