# Voxel Tools
The voxel tools offer ligthweight voxel geometry for Rhino. 
It allows you to quickly generate and manipulate voxelated geometry from meshes, breps, curves and points, and offers boolean operation between voxelgrids. It can convert voxelgrids to solid mesh hulls.

## Installation
**Rhino 6/7**
Use the yak package manager in Rhino 6 or Rhino 7, using the command `TestPackageManager` in Rhino 6 or `PackageManger` in Rhino 7, and search for VoxelTools

**Alternatively: Rhino 5/6/7**:
Download the VoxelTools.zip, and place it in `%appdata%/components`.

## Usage

### Voxelate geometry
- Closed mesh [01_closed_mesh.gh](examples/01_closed_mesh.gh)
- Open Mesh with attractor distance [02_closed_mesh.gh](examples/02_open_mesh.gh)
- Closed Brep [03_closed_brep.gh](examples/03_closed_brep.gh)
- Open Brep with attractor distance [04_open_brep.gh](examples/04_open_brep.gh)
- Curve with attractor distance [05_curve.gh](examples/05_curve.gh)
- Points with attractor distance [06_points.gh](examples/06_points.gh)

#### Boolean operations
For grids of the same size, you can do boolean operations. Boolean operations on voxelgrids are very quick and lightweight.
- Add
- Subtract
- XOR
- Invert 

#### Analysis
- Voxelgrid to boxes
- Voxelgrid statistics
- Voxelgrid to mesh hull

#### Serialization
- Export voxelgrid to string
- Import voxelgrid from string
- Voxelgrid to List
- Voxelgrid from List

## Need Help?
- Please use the github issue tracker if you find bugs
- For support please do not e-mail me personally, but find me (@arendvw) on the [McNeel forum](https://discourse.mcneel.com), use the tag `VoxelTools`

## Version History
### Voxeltools 1.0: release 25th of May 2020
- Cleaned up code base and merged old projects for open source release\
- Component GridToBoxes: Made both outputs consistent, added output selector
- Mesh casting: Default casting to mesh is now without false colors
- Mesh hull: Added non-default option to add false colors to the mesh
- Voxelate Meshes: Closed meshes are now voxelized much better
- Voxelate Curves: Curves are voxelated faster and more precise
- Voxelate Open Breps: Open breps can now also be voxelized with an attractor distance
- Improved warnings in marching cubes from boolean grid
- Added icons for grid to hex and hex to grid
- Obscured non-obvious (pixelgrid) or why factory specific components
- Added documentation

### Voxeltools 0.1: Released 2013-2014
Legacy VoxelTools - Legacy version that has circulated on the forums, The Why Factory and other places.
Use this version if somehow your older scripts are not compatible with the new version, but upgrade when you can.

## Licence
MIT Licence

## Roadmap
- Document scalar grids
- Document C# scripting usage using VoxelGeometry.dll
- Document Marching cubes
- Import/Export for voxbin common voxel formats
- Import/export 3d shapenet
- Rotation / movement of voxelgrids
- Union of non-uniformly sized grids
- Offset / Grow / Shrink

## Author
Arend van Waart, arend@studioavw.nl

## Thanks to
David Rutten, Sander Mulders, Huib Plomp, Adrien Ravon, Leo Stuckart, Boudewijn Thomas, Marek Nosek

### [The Why Factory](https://thewhyfactory.com/) Studio Porous Structures
Winy Maas, Alexander Sverdlov, Rob Nijsse, Bas Wijnbeld, Manthan Mevada, M.F. Hercules, Mitalee Parikh, Olga Berning, Peng Zhao, Xiao Du, Rudo Valentijn Koot, M.A. Heredia Moreno, Alberto González Ruiz, Narinna Gyulkhasyan

### [The Why Factory](https://thewhyfactory.com/) Studio Egocity
Winy Maas, Adrien Ravon, Felix Madrazo, Charles Ducerisier, Chun Hoi Hui, Francesco Barone, Félix Borel, Iason Stathatos, Javier López-Menchero Ortiz de Salazar, Lucile Dugal, Marek Nosek, Matteo Pavanello, Niels Baljet, Olga Terzi, Prokop Matej, Tarryn Leeferink, Wen Jun Tan, Woo Soojung, Zichen Liu, Loes Thijssen

## Publications
[PoroCity, Opening up Solidity](https://www.naibooksellers.nl/porocity-opening-up-solidity-the-why-factory.html)