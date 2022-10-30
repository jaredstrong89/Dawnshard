﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DragaliaAPI.Models.Dragalia.Responses.Common;
using DragaliaAPI.Models.Dragalia.Responses;
using MessagePack.Resolvers;
using MessagePack;

namespace DragaliaAPI.Controllers.Dragalia;

[Route("mypage")]
[Consumes("application/octet-stream")]
[Produces("application/octet-stream")]
[ApiController]
public class MypageController : ControllerBase
{
    [Route("info")]
    [HttpPost]
    public ActionResult<object> Info()
    {
        byte[] blob = System.IO.File.ReadAllBytes("Resources/mypage_info");
        dynamic preset_mypage = MessagePackSerializer.Deserialize<dynamic>(
            blob,
            ContractlessStandardResolver.Options
        );

        return Ok(preset_mypage);
    }
}