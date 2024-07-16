using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace DynamicSpace.Controllers;

public class GenericControllerActionDescriptorChangeProvider : IActionDescriptorChangeProvider
{
    public static GenericControllerActionDescriptorChangeProvider Instance { get; } = new GenericControllerActionDescriptorChangeProvider();

    public CancellationTokenSource? TokenSource { get; private set; }

    public bool HasChanged { get; set; }

    public IChangeToken GetChangeToken()
    {
        TokenSource = new CancellationTokenSource();
        return new CancellationChangeToken(TokenSource.Token);
    }
}
