extends Node

@export var character_body:CharacterBody2D
@export var sprite:Sprite2D
var is_invulnerable := false

func trigger(duration):
	delay(duration)
	character_body.modulate
#	character_body.set_collision_mask_value(1, false)

func delay(duration):
	is_invulnerable = true
	await get_tree().create_timer(duration).timeout
	is_invulnerable = false
