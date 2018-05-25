# deformablebodies-crowdsim-demo
This Unity project (C#) uses pseudo spring-mass forces to deform a body based on sound "force." The force is applied in a piecewise manner to each octant of a deformable jelly; it is dampened with spring forces which hold the jelly together. The video can be viewed [![here](https://youtu.be/_7lp90QAau8)](https://youtu.be/_7lp90QAau8)

Set timesetp t = 60 / beats-per-minute; expand with a force for each octant f = average amplitude over each frequency group over t; expand over half of t with a dampening force k_dampening, and apply 3-dimensional spring-mass forces to smooth expansion, detraction, and keep the jelly as a uniform body.
