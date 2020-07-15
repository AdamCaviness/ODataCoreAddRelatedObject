﻿// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

namespace Microsoft.Test.E2E.AspNet.OData.Formatter.JsonLight.Metadata.Model
{
    public class OneToOneParent
    {
        public int Id { get; set; }
        public OneToOneChild Child { get; set; }
    }
}
