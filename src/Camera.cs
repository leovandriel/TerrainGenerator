//==================================================================================================
//
// TerrainGenerator v.1 - To generate (natural looking) height maps of terrain.
//
// Copyright (C) 2008  Leo Vandriel
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//==================================================================================================

using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TerrainGenerator
{
    // Basic camera, including the view and projection transform and some movement and rotation
    // functions.
    public class Camera
    {
        private Device device = null;

        private Vector3 position = new Vector3(0f, 0f, 0f);
        private Vector3 look = new Vector3(0f, 0f, 1f);
        private Vector3 up = new Vector3(0f, 1f, 0f);
        private Vector3 right = new Vector3(1f, 0f, 0f);

        private Matrix projectionMatrix;


        // param setting

        private float stepSize = 1f;
        public float StepSize
        {
            get { return stepSize; }
            set { stepSize = value; }
        }

        private float angleSize = .01f;
        public float AngleSize
        {
            get { return angleSize; }
            set { angleSize = value; }
        }

        public Matrix View
        {
            get { return device.Transform.View; }
        }

        public Matrix Projection
        {
            get { return device.Transform.Projection; }
            set
            {
                projectionMatrix = value;
                device.Transform.Projection = value;
            }
        }

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateViewMatrix();
            }
        }

        public Vector3 Look
        {
            get { return look; }
            set
            {
                look = value;
                UpdateViewMatrix();
            }
        }

        public Vector3 Up
        {
            get { return up; }
            set
            {
                up = value;
                UpdateViewMatrix();
            }
        }


        // constructor

        public Camera(Device device)
        {
            this.device = device;
        }


        // update matrices

        public void UpdateViewMatrix()
        {
            look.Normalize();
            right = Vector3.Normalize(Vector3.Cross(look, up));
            up = Vector3.Normalize(Vector3.Cross(right, look));
            device.Transform.View = Matrix.LookAtLH(position, position+look, up);
        }

        public void UpdateProjectionMatrix()
        {
            device.Transform.Projection = projectionMatrix;
        }

        public void SetLookAtLH(Vector3 position, Vector3 lookAt, Vector3 up)
        {
            this.position = position;
            this.look = lookAt;
            this.up = up;
            UpdateViewMatrix();
        }


        // movement and rotation

        public void Move(Vector3 move)
        {
            position += stepSize * ((move.X * right) + (move.Y * up) + (move.Z * look));
            UpdateViewMatrix();
        }

        public void Rotate(int x, int y)
        {
            if (x != 0)
            {
                Matrix xRotation = Matrix.RotationAxis(new Vector3(0.0f, 1.0f, 0.0f), (float)x * angleSize);
                look = Vector3.TransformCoordinate(look, xRotation);
                up = Vector3.TransformCoordinate(up, xRotation);
                right = Vector3.TransformCoordinate(right, xRotation);
                UpdateViewMatrix();
            }
            if (y != 0)
            {
                Matrix yRotation = Matrix.RotationAxis(right, (float)-y * angleSize);
                look = Vector3.TransformCoordinate(look, yRotation);
                up = Vector3.TransformCoordinate(up, yRotation);
                UpdateViewMatrix();
            }
        }


    }
}
