using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace DynamicSpace.Controllers;

public class DynamicActionDescriptorChangeProvider : IActionDescriptorChangeProvider
{
    public static DynamicActionDescriptorChangeProvider Instance { get; } = new DynamicActionDescriptorChangeProvider();

    public CancellationTokenSource? TokenSource { get; private set; }

    public bool HasChanged { get; set; }

    public IChangeToken GetChangeToken()
    {
        TokenSource = new CancellationTokenSource();
        return new CancellationChangeToken(TokenSource.Token);
    }
}
