﻿using StereoKit;

class DemoGeo : IDemo
{
    Mesh  demoCubeMesh  = null;
    Model demoCubeModel = null;
    Mesh  demoRoundedCubeMesh  = null;
    Model demoRoundedCubeModel = null;
    Mesh  demoSphereMesh  = null;
    Model demoSphereModel = null;
    Mesh  demoCylinderMesh  = null;
    Model demoCylinderModel = null;
    Mesh  demoPlaneMesh  = null;
    Model demoPlaneModel = null;

    public void Initialize()
    {
        /// :CodeSample: Mesh.GenerateCube
        /// ![Procedural Geometry Demo]({{site.url}}/img/screenshots/ProceduralGeometry.jpg)
        /// Here's a quick example of generating a mesh! You can store it in just a
        /// Mesh, or you can attach it to a Model for easier rendering later on.
        Mesh  cubeMesh  = Mesh.GenerateCube(Vec3.One * 0.4f);
        Model cubeModel = Model.FromMesh(cubeMesh, Default.Material);
        /// :End:
        demoCubeMesh  = cubeMesh;
        demoCubeModel = cubeModel;

        /// :CodeSample: Mesh.GenerateRoundedCube
        /// ![Procedural Geometry Demo]({{site.url}}/img/screenshots/ProceduralGeometry.jpg)
        /// Here's a quick example of generating a mesh! You can store it in just a
        /// Mesh, or you can attach it to a Model for easier rendering later on.
        Mesh  roundedCubeMesh  = Mesh.GenerateRoundedCube(Vec3.One * 0.4f, 0.05f);
        Model roundedCubeModel = Model.FromMesh(roundedCubeMesh, Default.Material);
        /// :End:
        demoRoundedCubeMesh  = roundedCubeMesh;
        demoRoundedCubeModel = roundedCubeModel;

        /// :CodeSample: Mesh.GenerateSphere
        /// ![Procedural Geometry Demo]({{site.url}}/img/screenshots/ProceduralGeometry.jpg)
        /// Here's a quick example of generating a mesh! You can store it in just a
        /// Mesh, or you can attach it to a Model for easier rendering later on.
        Mesh  sphereMesh  = Mesh.GenerateSphere(0.4f);
        Model sphereModel = Model.FromMesh(sphereMesh, Default.Material);
        /// :End:
        demoSphereMesh  = sphereMesh;
        demoSphereModel = sphereModel;

        /// :CodeSample: Mesh.GenerateCylinder
        /// ![Procedural Geometry Demo]({{site.url}}/img/screenshots/ProceduralGeometry.jpg)
        /// Here's a quick example of generating a mesh! You can store it in just a
        /// Mesh, or you can attach it to a Model for easier rendering later on.
        Mesh  cylinderMesh  = Mesh.GenerateCylinder(0.4f, 0.4f, Vec3.Up);
        Model cylinderModel = Model.FromMesh(cylinderMesh, Default.Material);
        /// :End:
        demoCylinderMesh  = cylinderMesh;
        demoCylinderModel = cylinderModel;

        /// :CodeSample: Mesh.GeneratePlane
        /// ![Procedural Geometry Demo]({{site.url}}/img/screenshots/ProceduralGeometry.jpg)
        /// Here's a quick example of generating a mesh! You can store it in just a
        /// Mesh, or you can attach it to a Model for easier rendering later on.
        Mesh  planeMesh  = Mesh.GeneratePlane(Vec2.One*0.4f);
        Model planeModel = Model.FromMesh(planeMesh, Default.Material);
        /// :End:
        demoPlaneMesh  = planeMesh;
        demoPlaneModel = planeModel;
    }

    public void Shutdown()
    {
    }

    public void Update()
    {
        // Cube!
        Mesh  cubeMesh  = demoCubeMesh;
        Model cubeModel = demoCubeModel;

        if (Demos.TestMode)
            Renderer.Screenshot(new Vec3(0.25f,1.5f,2f)*0.75f, Vec3.Zero, 600, 400, "../../../docs/img/screenshots/ProceduralGeometry.jpg");

        /// :CodeSample: Mesh.GenerateCube
        /// Drawing both a Mesh and a Model generated this way is reasonably simple, 
        /// here's a short example! For the Mesh, you'll need to create your own material, 
        /// we just loaded up the default Material here.
        Matrix cubeTransform = Matrix.T(-1.0f, 0, 1);
        Renderer.Add(cubeMesh, Default.Material, cubeTransform);

        cubeTransform = Matrix.T(-1.0f, 0, -1);
        Renderer.Add(cubeModel, cubeTransform);
        /// :End:
        

        // Rounded Cube!
        Mesh  roundedCubeMesh  = demoRoundedCubeMesh;
        Model roundedCubeModel = demoRoundedCubeModel;

        /// :CodeSample: Mesh.GenerateRoundedCube
        /// Drawing both a Mesh and a Model generated this way is reasonably simple, 
        /// here's a short example! For the Mesh, you'll need to create your own material, 
        /// we just loaded up the default Material here.
        Matrix roundedCubeTransform = Matrix.T(-0.5f, 0, 1);
        Renderer.Add(roundedCubeMesh, Default.Material, roundedCubeTransform);

        roundedCubeTransform = Matrix.T(-0.5f, 0, -1);
        Renderer.Add(roundedCubeModel, roundedCubeTransform);
        /// :End:


        // Rounded Sphere!
        Mesh  sphereMesh  = demoSphereMesh;
        Model sphereModel = demoSphereModel;

        /// :CodeSample: Mesh.GenerateSphere
        /// Drawing both a Mesh and a Model generated this way is reasonably simple, 
        /// here's a short example! For the Mesh, you'll need to create your own material, 
        /// we just loaded up the default Material here.
        Matrix sphereTransform = Matrix.T(0.0f, 0, 1);
        Renderer.Add(sphereMesh, Default.Material, sphereTransform);

        sphereTransform = Matrix.T(0.0f, 0, -1);
        Renderer.Add(sphereModel, sphereTransform);
        /// :End:


        // Cylinder!
        Mesh  cylinderMesh  = demoCylinderMesh;
        Model cylinderModel = demoCylinderModel;

        /// :CodeSample: Mesh.GenerateCylinder
        /// Drawing both a Mesh and a Model generated this way is reasonably simple, 
        /// here's a short example! For the Mesh, you'll need to create your own material, 
        /// we just loaded up the default Material here.
        Matrix cylinderTransform = Matrix.T(0.5f, 0, 1);
        Renderer.Add(cylinderMesh, Default.Material, cylinderTransform);

        cylinderTransform = Matrix.T(0.5f, 0, -1);
        Renderer.Add(cylinderModel, cylinderTransform);
        /// :End:


        // Plane!
        Mesh  planeMesh  = demoPlaneMesh;
        Model planeModel = demoPlaneModel;

        /// :CodeSample: Mesh.GeneratePlane
        /// Drawing both a Mesh and a Model generated this way is reasonably simple, 
        /// here's a short example! For the Mesh, you'll need to create your own material, 
        /// we just loaded up the default Material here.
        Matrix planeTransform = Matrix.T(1.0f, 0, 1);
        Renderer.Add(planeMesh, Default.Material, planeTransform);

        planeTransform = Matrix.T(1.0f, 0, -1);
        Renderer.Add(planeModel, planeTransform);
        /// :End:
    }
}
