extends Area2D

func _on_body_entered(body):
	if body.is_in_group("player"):
		body.hit(1)
		var offset = -1 if body.data.last_ground_position.x < position.x else 1 
		var last_ground_position = Vector2(
			body.data.last_ground_position.x + (body.collision.shape.get_rect().size.x * offset),
			body.data.last_ground_position.y
		)
		body.position = last_ground_position
