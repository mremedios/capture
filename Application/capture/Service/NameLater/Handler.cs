using System;
using System.IO;
using Capture.Service.Database;
using Capture.Service.Parser;
using Microsoft.Extensions.Logging;

namespace Capture.Service.NameLater;

public class Handler : IHandler
{

     private readonly ILogger<Handler> _logger;
     private readonly JsonContext _context;

     public Handler(ILogger<Handler> logger, JsonContext context)
     {
          _logger = logger;
          _context = context;
     }

     public async void HandleMessage(byte[] msg)
     {
          try
          {
               var message = ParserHePv3.ParseMessage(msg);
               var x = new Header
               {
                    raw = System.Text.Encoding.Default.GetString(message.Payload),
                    protocol_header = message.Headers
               };
               x.protocol_header["call-id"] = message.Sip.CallId; // скорее всего там уже есть такой хедер :/
               // + host ...
               await _context.AddAsync(x);
               _context.SaveChanges(); // async все ломает?
               _logger.LogInformation("Put header with callId {}",  message.Sip.CallId);
               
          }
          catch (IOException e)
          {
               _logger.LogWarning(e.Message);
          }
     }
}
