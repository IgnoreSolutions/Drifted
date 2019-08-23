/*
    Copyright (C) 2019 Mike Santiago - All Rights Reserved
    axiom@ignoresolutions.xyz

    Permission to use, copy, modify, and/or distribute this software for any
    purpose with or without fee is hereby granted, provided that the above
    copyright notice and this permission notice appear in all copies.

    THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
    WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
    MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
    ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
    WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
    ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
    OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Drifted;
using Drifted.Items.Next;
using Drifted.UI;
using UnityEngine;

[Serializable]
public class ItemContainer
{
    [SerializeField]
    Item _item = null;
    public int Quantity = 0;
    

    [NonSerialized]
    private GameEventListener gel;

    public ItemContainer()
    {
        _item = null;
        Quantity = 0;
    }

    public ItemContainer(Item item, int quantity = 1)
    {
        _item = item;
        Quantity = quantity;
    }

    public Item GetItem() => _item;
    public void SetItem(Item item)
    {
        _item = item;
        if (_item == null) Quantity = 0;
    }


}
