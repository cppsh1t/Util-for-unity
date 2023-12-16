using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtil.BehaviourTree.Editor
{
    public class XmlField : ObjectField
    {
        public new class UxmlFactory : UxmlFactory<XmlField, UxmlTraits> { }
    }
}