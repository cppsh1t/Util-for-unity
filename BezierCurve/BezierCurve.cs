using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil.BezierCurve
{
    public interface IBezierCurve
    {
        public Vector3 Bezier(float t);
    }

    public class TransformBezierCurve : IBezierCurve
    {
        private Transform[] transforms;
        private Vector3[] calcPos;

        public TransformBezierCurve(Transform[] transforms)
        {
            this.transforms = transforms;
            calcPos = new Vector3[transforms.Length];
        }

        public void SetTransforms(Transform[] transforms)
        {
            if (transforms.Length != this.transforms.Length)
            {
                calcPos = new Vector3[transforms.Length];
            }
            this.transforms = transforms;
        }

        public Vector3 Bezier(float t)
        {
            t = Mathf.Clamp01(t);
            int n = transforms.Length - 1;

            for (int i = 0; i <= n; i++)
            {
                calcPos[i] = transforms[i].position;
            }

            for (int r = 1; r <= n; r++)
            {
                for (int i = 0; i <= n - r; i++)
                {
                    calcPos[i] = (1 - t) * calcPos[i] + t * calcPos[i + 1];
                }
            }

            return calcPos[0];
        }
    }

    public class PositionBezierCurve : IBezierCurve
    {
        private Vector3[] positions;
        private Vector3[] calcPos;

        public PositionBezierCurve(Vector3[] positions)
        {
            this.positions = positions;
            calcPos = new Vector3[positions.Length];
        }

        public void SetTransforms(Vector3[] positions)
        {
            if (this.positions.Length != positions.Length)
            {
                calcPos = new Vector3[positions.Length];
            }
            this.positions = positions;
        }

        public Vector3 Bezier(float t)
        {
            t = Mathf.Clamp01(t);
            int n = positions.Length - 1;
            Array.Copy(positions, calcPos, positions.Length);
            for (int r = 1; r <= n; r++)
            {
                for (int i = 0; i <= n - r; i++)
                {
                    calcPos[i] = (1 - t) * calcPos[i] + t * calcPos[i + 1];
                }
            }

            return calcPos[0];
        }
    }

    public class AutoTransformBezierCurve : IBezierCurve
    {
        private Transform[] transforms;
        private Vector3[] calcPos;
        private Vector3 startPos;

        public AutoTransformBezierCurve(Vector3 startPos, Transform[] transforms)
        {
            this.startPos = startPos;
            this.transforms = transforms;
            calcPos = new Vector3[transforms.Length + 1];
        }

        public void SetStartPosition(Vector3 startPos)
        {
            this.startPos = startPos;
        }

        public void SetTransforms(Transform[] transforms)
        {
            if (transforms.Length != this.transforms.Length)
            {
                calcPos = new Vector3[transforms.Length + 1];
            }
            this.transforms = transforms;
        }

        public Vector3 Bezier(float t)
        {
            t = Mathf.Clamp01(t);
            int n = transforms.Length;

            for (int i = 0; i <= n; i++)
            {
                if (i == 0)
                {
                    calcPos[i] = startPos;
                }
                else
                {
                    calcPos[i] = transforms[i - 1].position;
                }
            }

            for (int r = 1; r <= n; r++)
            {
                for (int i = 0; i <= n - r; i++)
                {
                    calcPos[i] = (1 - t) * calcPos[i] + t * calcPos[i + 1];
                }
            }

            return calcPos[0];
        }
    }

}