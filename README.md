# ODataCoreAddRelatedObject
Demonstrates an issue with aspnetcore WebApi OData library when a client calls AddRelatedObject and the POST body contains ContentIds like $1.

## Please read the related Microsoft.AspNetCore.OData issue
[https://github.com/OData/WebApi/issues/2071](https://github.com/OData/WebApi/issues/2071)

## Inspect the fix in the Fix branch
[https://github.com/AdamCaviness/ODataCoreAddRelatedObject/tree/Fix](https://github.com/AdamCaviness/ODataCoreAddRelatedObject/tree/Fix)

## Discovery
The crux of the issue is that ContentIdHelpers.ResolveContentId() requires a url, not a querystring. It therefore returned an empty string which resulted in never calling context.Request.CopyAbsoluteUrl() which prevented the request's path from being updated from the ContentId syntax (like $1) to the expanded uri syntax. You may compare this with [my aspnet framework sample](https://github.com/AdamCaviness/ODataAddRelatedObject) to see what was done in ODataBatchRequestItem there.
