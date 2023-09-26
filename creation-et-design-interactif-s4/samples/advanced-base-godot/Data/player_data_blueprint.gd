extends Resource

class_name PlayerData

@export var max_health: int
@export var current_health: int
@export var speed: int
@export var jump_height: int
@export var max_jump: int
@export var invulnerable_time: int = 3
var initial_spawn_position: Vector2 = Vector2.ZERO
var current_spawn_position: Vector2 = Vector2.ZERO
var last_ground_position: Vector2 = Vector2.ZERO

signal on_death
