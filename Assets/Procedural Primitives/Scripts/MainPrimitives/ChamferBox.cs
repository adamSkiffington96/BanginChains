﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralPrimitivesUtil
{
    public class ChamferBox : PPBase
    {
        [Header("Basic parameters")]
        public float width = 1.0f;
        public float length = 1.0f;
        public float height = 1.0f;
        public float fillet = 0.1f;
        public bool flatTop = false;
        public bool flatBottom = false;
        [Header("Segments")]
        public int widthSegs = 2;
        public int lengthSegs = 2;
        public int heightSegs = 2;
        public int filletSegs = 3;
        [Header("Mapping Coordinates")]
        public bool generateMappingCoords = true;
        public bool realWorldMapSize = false;
        public Vector2 UVOffset = new Vector2(0.0f, 0.0f);
        public Vector2 UVTiling = new Vector2(1.0f, 1.0f);
        [Header("Others")]
        public bool flipNormals = false;
        public bool smooth = true;

        protected override void CreateMesh()
        {
            m_mesh.name = "Chamfer Box";

            length = Mathf.Clamp(length, 0.00001f, 10000.0f);
            width = Mathf.Clamp(width, 0.00001f, 10000.0f);
            height = Mathf.Clamp(height, 0.00001f, 10000.0f);
            float min = length < width ? length : width;
            min = min < height ? min : height;
            fillet = Mathf.Clamp(fillet, 0.00001f, min / 2.0f);
            lengthSegs = Mathf.Clamp(lengthSegs, 1, 100);
            widthSegs = Mathf.Clamp(widthSegs, 1, 100);
            heightSegs = Mathf.Clamp(heightSegs, 1, 100);
            filletSegs = Mathf.Clamp(filletSegs, 1, 100);

            float lengthHalf = length * 0.5f;
            float widthHalf = width * 0.5f;
            float heightHalf = height * 0.5f;
            float filletHalf = fillet * 0.5f;
            float lengthHalfFillet = lengthHalf - fillet;
            float widthHalfFillet = widthHalf - fillet;
            float heightHalfFillet = heightHalf - fillet;
            float lengthHalfFilletHalf = lengthHalf - filletHalf;
            float widthHalfFilletHalf = widthHalf - filletHalf;
            float lengthFillet = lengthHalfFillet * 2.0f;
            float widthFillet = widthHalfFillet * 2.0f;
            float heightFillet = heightHalfFillet * 2.0f;
            float heightCenter = 0.0f;

            Vector2 tilingCylinder = realWorldMapSize ? new Vector2(1.0f, 1.0f) : new Vector2(0.25f, 1.0f);
            Vector2 tilingSphere = realWorldMapSize ? new Vector2(1.0f, 1.0f) : new Vector2(0.25f, 1.0f);
            tilingCylinder = new Vector2(UVTiling.x * tilingCylinder.x, UVTiling.y * tilingCylinder.y);
            tilingSphere = new Vector2(UVTiling.x * tilingSphere.x, UVTiling.y * tilingSphere.y);

            Vector2 tilingCircle = realWorldMapSize ? new Vector2(1.0f, 1.0f) : new Vector2(fillet / widthHalf, fillet / lengthHalf);
            tilingCircle = new Vector2(UVTiling.x * tilingCircle.x, UVTiling.y * tilingCircle.y);
            Vector2 tilingcenter = realWorldMapSize ? new Vector2(1.0f, 1.0f) : new Vector2(widthHalfFillet / widthHalf, lengthHalfFillet / lengthHalf);
            tilingcenter = new Vector2(UVTiling.x * tilingcenter.x, UVTiling.y * tilingcenter.y);
            Vector2 tilinglr = realWorldMapSize ? new Vector2(1.0f, 1.0f) : new Vector2(fillet / width, lengthHalfFillet / lengthHalf);
            tilinglr = new Vector2(UVTiling.x * tilinglr.x, UVTiling.y * tilinglr.y);
            Vector2 tilingtb = realWorldMapSize ? new Vector2(1.0f, 1.0f) : new Vector2(widthHalfFillet / widthHalf, fillet / length);
            tilingtb = new Vector2(UVTiling.x * tilingtb.x, UVTiling.y * tilingtb.y);

            //up
            if (flatTop)
            {
                //center
                CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, heightHalf, 0.0f), m_rotation * Vector3.forward, m_rotation * Vector3.right, widthFillet, lengthFillet, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(filletHalf / widthHalf, filletHalf / lengthHalf), tilingcenter, flipNormals);

                //top bottom
                CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, heightHalf, lengthHalfFilletHalf), m_rotation * Vector3.forward, m_rotation * Vector3.right, widthFillet, fillet, widthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(filletHalf / widthHalf, (length - fillet) / length), tilingtb, flipNormals);
                CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, heightHalf, -lengthHalfFilletHalf), m_rotation * Vector3.forward, m_rotation * Vector3.right, widthFillet, fillet, widthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(filletHalf / widthHalf, 0.0f), tilingtb, flipNormals);

                //left right
                CreatePlane(pivotOffset + m_rotation * new Vector3(-widthHalfFilletHalf, heightHalf, 0.0f), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, lengthFillet, filletSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.0f, filletHalf / lengthHalf), tilinglr, flipNormals);
                CreatePlane(pivotOffset + m_rotation * new Vector3(widthHalfFilletHalf, heightHalf, 0.0f), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, lengthFillet, filletSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2((width - fillet) / width, filletHalf / lengthHalf), tilinglr, flipNormals);

                //corner
                CreateCircle(pivotOffset + m_rotation * new Vector3(widthHalf - fillet, heightHalf, lengthHalf - fillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs, true, 0.0f, 90.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(widthHalfFillet / width, lengthHalfFillet / length), tilingCircle, flipNormals);
                CreateCircle(pivotOffset + m_rotation * new Vector3(widthHalf - fillet, heightHalf, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(widthHalfFillet / width, -lengthHalfFillet / length), tilingCircle, flipNormals);
                CreateCircle(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, heightHalf, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(-widthHalfFillet / width, -lengthHalfFillet / length), tilingCircle, flipNormals);
                CreateCircle(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, heightHalf, lengthHalf - fillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(-widthHalfFillet / width, lengthHalfFillet / length), tilingCircle, flipNormals);

                heightFillet += fillet;
                heightCenter += fillet * 0.5f;
            }
            else
            {
                CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, heightHalf, 0.0f), m_rotation * Vector3.forward, m_rotation * Vector3.right, widthFillet, lengthFillet, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                CreateCylinder(pivotOffset + m_rotation * new Vector3(widthHalfFillet, heightHalfFillet, 0.0f), m_rotation * Vector3.down, m_rotation * Vector3.right, lengthFillet, fillet, filletSegs, lengthSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.25f, 0.0f), tilingCylinder, flipNormals, smooth);
                CreateCylinder(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, heightHalfFillet, 0.0f), m_rotation * Vector3.down, m_rotation * Vector3.right, lengthFillet, fillet, filletSegs, lengthSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.5f, 0.0f), tilingCylinder, flipNormals, smooth);
                CreateCylinder(pivotOffset + m_rotation * new Vector3(0.0f, heightHalfFillet, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.down, widthFillet, fillet, filletSegs, widthSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.5f, 0.0f), tilingCylinder, flipNormals, smooth);
                CreateCylinder(pivotOffset + m_rotation * new Vector3(0.0f, heightHalfFillet, lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.down, widthFillet, fillet, filletSegs, widthSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.75f, 0.0f), tilingCylinder, flipNormals, smooth);
                CreateSphere(pivotOffset + m_rotation * new Vector3(widthHalfFillet, heightHalfFillet, lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs * 2, true, 0.0f, 90.0f, true, Mathf.PI * 0.5f, Mathf.PI, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.0f, 0.0f), tilingSphere, flipNormals, smooth);
                CreateSphere(pivotOffset + m_rotation * new Vector3(widthHalfFillet, heightHalfFillet, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs * 2, true, 90.0f, 180.0f, true, Mathf.PI * 0.5f, Mathf.PI, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.25f, 0.0f), tilingSphere, flipNormals, smooth);
                CreateSphere(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, heightHalfFillet, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs * 2, true, 180.0f, 270.0f, true, Mathf.PI * 0.5f, Mathf.PI, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.5f, 0.0f), tilingSphere, flipNormals, smooth);
                CreateSphere(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, heightHalfFillet, lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs * 2, true, 270.0f, 360.0f, true, Mathf.PI * 0.5f, Mathf.PI, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.75f, 0.0f), tilingSphere, flipNormals, smooth);
            }

            //down
            if (flatBottom)
            {
                //center
                CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, -heightHalf, 0.0f), m_rotation * Vector3.forward, m_rotation * Vector3.left, widthFillet, lengthFillet, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(filletHalf / widthHalf, filletHalf / lengthHalf), tilingcenter, flipNormals);

                //top bottom
                CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, -heightHalf, lengthHalfFilletHalf), m_rotation * Vector3.forward, m_rotation * Vector3.left, widthFillet, fillet, widthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(filletHalf / widthHalf, (length - fillet) / length), tilingtb, flipNormals);
                CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, -heightHalf, -lengthHalfFilletHalf), m_rotation * Vector3.forward, m_rotation * Vector3.left, widthFillet, fillet, widthSegs, filletSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(filletHalf / widthHalf, 0.0f), tilingtb, flipNormals);

                //left right
                CreatePlane(pivotOffset + m_rotation * new Vector3(-widthHalfFilletHalf, -heightHalf, 0.0f), m_rotation * Vector3.forward, m_rotation * Vector3.left, fillet, lengthFillet, filletSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2((width - fillet) / width, filletHalf / lengthHalf), tilinglr, flipNormals);
                CreatePlane(pivotOffset + m_rotation * new Vector3(widthHalfFilletHalf, -heightHalf, 0.0f), m_rotation * Vector3.forward, m_rotation * Vector3.left, fillet, lengthFillet, filletSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.0f, filletHalf / lengthHalf), tilinglr, flipNormals);

                //corner
                CreateCircle(pivotOffset + m_rotation * new Vector3(widthHalf - fillet, -heightHalf, lengthHalf - fillet), m_rotation * Vector3.forward, m_rotation * Vector3.left, fillet, filletSegs, filletSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(-widthHalfFillet / width, lengthHalfFillet / length), tilingCircle, flipNormals);
                CreateCircle(pivotOffset + m_rotation * new Vector3(widthHalf - fillet, -heightHalf, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.left, fillet, filletSegs, filletSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(-widthHalfFillet / width, -lengthHalfFillet / length), tilingCircle, flipNormals);
                CreateCircle(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, -heightHalf, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.left, fillet, filletSegs, filletSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(widthHalfFillet / width, -lengthHalfFillet / length), tilingCircle, flipNormals);
                CreateCircle(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, -heightHalf, lengthHalf - fillet), m_rotation * Vector3.forward, m_rotation * Vector3.left, fillet, filletSegs, filletSegs, true, 0.0f, 90.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(widthHalfFillet / width, lengthHalfFillet / length), tilingCircle, flipNormals);
                
                heightFillet += fillet;
                heightCenter -= fillet * 0.5f;
            }
            else
            {
                CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, -heightHalf, 0.0f), m_rotation * Vector3.forward, m_rotation * Vector3.left, widthFillet, lengthFillet, widthSegs, lengthSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
                CreateCylinder(pivotOffset + m_rotation * new Vector3(0.0f, -heightHalfFillet, lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.down, widthFillet, fillet, filletSegs, widthSegs, true, 0.0f, 90.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.0f, 0.0f), tilingCylinder, flipNormals, smooth);
                CreateCylinder(pivotOffset + m_rotation * new Vector3(0.0f, -heightHalfFillet, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.down, widthFillet, fillet, filletSegs, widthSegs, true, 90.0f, 180.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.25f, 0.0f), tilingCylinder, flipNormals, smooth);
                CreateCylinder(pivotOffset + m_rotation * new Vector3(widthHalfFillet, -heightHalfFillet, 0.0f), m_rotation * Vector3.down, m_rotation * Vector3.right, lengthFillet, fillet, filletSegs, lengthSegs, true, 0.0f, 90.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.0f, 0.0f), tilingCylinder, flipNormals, smooth);
                CreateCylinder(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, -heightHalfFillet, 0.0f), m_rotation * Vector3.down, m_rotation * Vector3.right, lengthFillet, fillet, filletSegs, lengthSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.75f, 0.0f), tilingCylinder, flipNormals, smooth);
                CreateSphere(pivotOffset + m_rotation * new Vector3(widthHalfFillet, -heightHalfFillet, lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs * 2, true, 0.0f, 90.0f, true, 0.0f, Mathf.PI * 0.5f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.0f, 0.0f), tilingSphere, flipNormals, smooth);
                CreateSphere(pivotOffset + m_rotation * new Vector3(widthHalfFillet, -heightHalfFillet, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs * 2, true, 90.0f, 180.0f, true, 0.0f, Mathf.PI * 0.5f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.25f, 0.0f), tilingSphere, flipNormals, smooth);
                CreateSphere(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, -heightHalfFillet, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs * 2, true, 180.0f, 270.0f, true, 0.0f, Mathf.PI * 0.5f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.5f, 0.0f), tilingSphere, flipNormals, smooth);
                CreateSphere(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, -heightHalfFillet, lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, fillet, filletSegs, filletSegs * 2, true, 270.0f, 360.0f, true, 0.0f, Mathf.PI * 0.5f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.75f, 0.0f), tilingSphere, flipNormals, smooth);
            }

            //surrounding
            CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, heightCenter, -lengthHalf), m_rotation * Vector3.up,      m_rotation * Vector3.right,   widthFillet,  heightFillet, widthSegs,  heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreatePlane(pivotOffset + m_rotation * new Vector3(0.0f, heightCenter, lengthHalf),  m_rotation * Vector3.up,      m_rotation * Vector3.left,    widthFillet,  heightFillet, widthSegs,  heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreatePlane(pivotOffset + m_rotation * new Vector3(-widthHalf, heightCenter, 0.0f),  m_rotation * Vector3.up,      m_rotation * Vector3.back,    lengthFillet, heightFillet, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            CreatePlane(pivotOffset + m_rotation * new Vector3(widthHalf, heightCenter, 0.0f),   m_rotation * Vector3.up,      m_rotation * Vector3.forward, lengthFillet, heightFillet, lengthSegs, heightSegs, generateMappingCoords, realWorldMapSize, UVOffset, UVTiling, flipNormals);
            
            CreateCylinder(pivotOffset + m_rotation * new Vector3(widthHalfFillet, heightCenter, lengthHalfFillet),   m_rotation * Vector3.forward, m_rotation * Vector3.right, heightFillet, fillet, filletSegs, heightSegs, true, 0.0f,   90.0f,  generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.0f, 0.0f),  tilingCylinder, flipNormals, smooth);
            CreateCylinder(pivotOffset + m_rotation * new Vector3(widthHalfFillet, heightCenter, -lengthHalfFillet),  m_rotation * Vector3.forward, m_rotation * Vector3.right, heightFillet, fillet, filletSegs, heightSegs, true, 90.0f,  180.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.25f, 0.0f), tilingCylinder, flipNormals, smooth);
            CreateCylinder(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, heightCenter, -lengthHalfFillet), m_rotation * Vector3.forward, m_rotation * Vector3.right, heightFillet, fillet, filletSegs, heightSegs, true, 180.0f, 270.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.5f, 0.0f),  tilingCylinder, flipNormals, smooth);
            CreateCylinder(pivotOffset + m_rotation * new Vector3(-widthHalfFillet, heightCenter, lengthHalfFillet),  m_rotation * Vector3.forward, m_rotation * Vector3.right, heightFillet, fillet, filletSegs, heightSegs, true, 270.0f, 360.0f, generateMappingCoords, realWorldMapSize, UVOffset + new Vector2(0.75f, 0.0f), tilingCylinder, flipNormals, smooth);
        }
    }
}