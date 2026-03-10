# 🚂 Synthetic Train Loading Data Generator

A **synthetic data generator for object detection in railway wagons**,
developed as part of the master's dissertation:

**"Object Detection Based on Synthetic Data for Railway Transport: YOLO
vs RT-DETR in Train Loading Operations."**

The goal of this project is to generate **realistic synthetic datasets
for training computer vision models**, simulating industrial scenarios
where empty wagons must be inspected before the ore loading process.

The system uses **3D simulation with Unity and Blender** to create
railway environments under different operating conditions, enabling the
automatic generation of **images and YOLO-format annotations**.

------------------------------------------------------------------------

# 📷 Simulated Environment Example

![Unity simulated scene](docs/images/unity_scene.png)

Example of a virtual scene used to generate the synthetic dataset.

------------------------------------------------------------------------

# 🎯 Motivation

During railway loading operations, the presence of **foreign objects
inside empty wagons** may cause:

-   contamination of transported material
-   equipment damage
-   operational safety risks

However, collecting real data in industrial environments is challenging
due to:

-   operational restrictions
-   industrial safety constraints
-   data privacy concerns
-   difficulty capturing rare events

For this reason, this project uses **3D simulation to generate realistic
synthetic datasets**, enabling object detection model training without
relying on real industrial data.

------------------------------------------------------------------------

# 🧠 System Overview

The synthetic data generator pipeline consists of four main stages:

1.  **3D modeling of scene elements**
2.  **Construction of the railway environment**
3.  **Random instantiation of objects**
4.  **Automatic image capture and annotation generation**

------------------------------------------------------------------------

# 🧱 3D Modeling

Scene assets were created or adapted using **Blender**.

These include:

-   railway wagons
-   rails
-   loading infrastructure
-   terrain and vegetation
-   foreign objects

![Object modeled in Blender](docs/images/blender_object.png)

Example of a modeled and textured object used in the simulation.

------------------------------------------------------------------------

# 🌎 Simulation Environment

The virtual environment was developed using the **Unity game engine**,
enabling the simulation of a complete railway loading scenario.

The system includes:

-   railway tracks and wagons
-   textured terrain
-   dynamic lighting
-   weather effects

![First simulation prototype](docs/images/first_simulation.png)

First proof‑of‑concept of the simulated loading environment.

------------------------------------------------------------------------

# 🌦️ Simulated Environmental Conditions

The system can generate data under multiple operational conditions:

-   ☀️ Day
-   🌙 Night
-   🌧️ Rain
-   🌧️🌙 Rainy Night

These variations increase dataset diversity and improve model
generalization.

------------------------------------------------------------------------

# 📦 Objects Inserted in Wagons

Objects used in the simulation were divided into three categories:

### Inanimate Objects

-   boxes
-   bottles
-   tires
-   cones
-   pallets

### Animals

-   dogs
-   cats
-   birds
-   snakes

### Humans

A total of **200 different 3D models** were used:

-   100 inanimate objects
-   50 humans
-   50 animals

![Exemple of objects](docs/images/objects.png)

Example of objects inserted in wagons.

------------------------------------------------------------------------

# 🎲 Random Object Placement

During simulation:

-   each wagon contains **predefined object spawn locations**
-   **1 to 3 objects may appear in each wagon**
-   position and rotation are determined using Unity physics

Probability distribution:

  Number of objects   Probability
  ------------------- -------------
  -   1                   100%
  -   2                   50%
  -   3                   25%

------------------------------------------------------------------------

# 📸 Automatic Image Capture

A camera positioned above the scene automatically captures the images.

Configuration:

-   resolution: **1080×1080**
-   fixed camera
-   field of view adjusted for the industrial scene

Post‑processing effects used:

-   Bloom
-   Motion Blur
-   Ambient Occlusion
-   Vignette

These effects increase the visual realism of the generated images.

------------------------------------------------------------------------

# 🏷️ Automatic Label Generation

Bounding boxes are generated automatically using the **3D geometry of
the objects**.

Process:

1.  collect object mesh vertices
2.  convert coordinates to camera space
3.  compute bounding box limits
4.  visibility verification
5.  export annotations in **YOLO format**

![Finished generator](docs/images/finished_generator.png)

Finished synthetic data generator.

------------------------------------------------------------------------

# 📊 Generated Dataset

The generator produced:

-   **33,000 synthetic images**
-   automatically generated annotations
-   **5‑fold cross‑validation split**

Distribution:

  Fold     Number of Images
  -------- ------------------
  -   Fold 1   6600
  -   Fold 2   6600
  -   Fold 3   6600
  -   Fold 4   6600
  -   Fold 5   6600

![Examples of generated data](docs/images/generated_data.png)

Examples of generated data.

------------------------------------------------------------------------

# 🤖 Trained Models

The generated data was used to train the following object detection
models:

-   **YOLOv10‑l**
-   **YOLOv10‑x**
-   **RT‑DETR‑l**
-   **RT‑DETR‑x**

Evaluation metrics:

-   **mAP@50**
-   **mAP@50‑95**

------------------------------------------------------------------------

# 🧰 Technologies Used

-   Unity
-   Blender
-   C#
-   Python
-   YOLOv10
-   RT‑DETR
-   Ultralytics

------------------------------------------------------------------------

# 🚀 Possible Applications

-   automated wagon inspection
-   foreign object detection
-   industrial computer vision systems
-   synthetic dataset generation
-   artificial intelligence research

------------------------------------------------------------------------

# 📚 Citation

If you use this project in academic research, please cite:

Thiago Leonardo Maria. Object Detection Based on Synthetic Data for
Railway Transport: YOLO vs RT‑DETR in Train Loading Operations. Master's
Dissertation -- Federal University of Ouro Preto.

------------------------------------------------------------------------

# 📄 License

This project is available for **academic and research purposes**.
